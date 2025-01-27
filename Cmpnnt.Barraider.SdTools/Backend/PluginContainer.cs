using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BarRaider.SdTools.Communication;
using BarRaider.SdTools.Communication.SDEvents;
using BarRaider.SdTools.Payloads;
using BarRaider.SdTools.StreamDeckInfo;
using BarRaider.SdTools.Utilities;
using BarRaider.SdTools.Wrappers;
using Newtonsoft.Json.Linq;

namespace BarRaider.SdTools.Backend
{
    // TODO: Replace async void with async Task
    internal class PluginContainer(IPluginActionRegistry actionRegistry, IUpdateHandler updateHandler)
    {
        private const int STREAMDECK_INITIAL_CONNECTION_TIMEOUT_SECONDS = 60;
        private StreamDeckConnection connection;
        private readonly ManualResetEvent connectEvent = new(false);
        private readonly ManualResetEvent disconnectEvent = new(false);
        private readonly SemaphoreSlim instancesLock = new(1);
        private string pluginUuid;
        private RegistrationInfo deviceInfo;
        private PluginUpdateInfo lastUpdateInfo;

        /// <summary>
        /// All current instances of plugin actions. Keyed on Action UUID. Action instances are added
        /// when the StreamDeck page containing the plugin is shown and removed when it is not visible.
        /// See the method Connection_OnWillAppear() in this file for that logic.
        
        /// Note that your _plugin_ will still run on the user's machine as long as it is installed, even if
        /// there are no actions running. So, this logic keeps resource usage down when no actions are running.
        /// </summary>
        private static readonly Dictionary<string, ICommonPluginFunctions> Instances = new();

        public void Run(StreamDeckOptions options)
        {
            pluginUuid = options.PluginUuid;
            deviceInfo = options.DeviceInfo;
            connection = new StreamDeckConnection(options.Port, options.PluginUuid, options.RegisterEvent);
            
            // Register for events
            connection.OnConnected += Connection_OnConnected;
            connection.OnDisconnected += Connection_OnDisconnected;
            connection.OnKeyDown += Connection_OnKeyDown;
            connection.OnKeyUp += Connection_OnKeyUp;
            connection.OnWillAppear += Connection_OnWillAppear;
            connection.OnWillDisappear += Connection_OnWillDisappear;
            connection.OnDialRotate += Connection_OnDialRotate;
            connection.OnDialDown += Connection_OnDialDown;
            connection.OnDialUp += Connection_OnDialUp;
            connection.OnTouchpadPress += Connection_OnTouchpadPress;

            // Settings changed
            connection.OnDidReceiveSettings += Connection_OnDidReceiveSettings;
            connection.OnDidReceiveGlobalSettings += Connection_OnDidReceiveGlobalSettings;

            // Plugin Version Updates
            if (updateHandler != null)
            {
                updateHandler.OnUpdateStatusChanged += UpdateHandler_OnUpdateStatusChanged;
                updateHandler.SetPluginConfiguration(Tools.GetExeName(), deviceInfo.Plugin.Version);
                Task.Run(() => updateHandler.CheckForUpdate());
            }

            // Start the connection
            connection.Run();
            Logger.Instance.LogMessage(TracingLevel.Debug, $"Plugin Loaded: UUID: {pluginUuid} Device Info: {deviceInfo}");
            Logger.Instance.LogMessage(TracingLevel.Info, $"Plugin version: {deviceInfo.Plugin.Version}");
            Logger.Instance.LogMessage(TracingLevel.Info, "Connecting to Stream Deck...");

            // Time to wait for initial connection
            if (connectEvent.WaitOne(TimeSpan.FromSeconds(STREAMDECK_INITIAL_CONNECTION_TIMEOUT_SECONDS)))
            {
                Logger.Instance.LogMessage(TracingLevel.Info, "Connected to Stream Deck");

                // Initialize GlobalSettings manager
                GlobalSettingsManager.Instance.Initialize(connection);

                // We connected, loop every second until we disconnect
                // TODO: Is there a better way to do this than a busy-wait? Some built-in background
                // Task that is delayed via Task.Delay? I don't like hogging a thread for this.
                while (!disconnectEvent.WaitOne(TimeSpan.FromMilliseconds(1000)))
                {
                    RunTick();
                }
            }
            Logger.Instance.LogMessage(TracingLevel.Info, "Plugin Disconnected - Exiting");
        }

        // Button pressed
        private async void Connection_OnKeyDown(object sender, SdEventReceivedEventArgs<KeyDownEvent> e)
        {
            if (updateHandler?.IsBlockingUpdate ?? false)
            {
                if (!string.IsNullOrEmpty(lastUpdateInfo?.UpdateUrl))
                {
                    await connection.OpenUrlAsync(lastUpdateInfo.UpdateUrl);
                }
                return;
            }

            await instancesLock.WaitAsync();
            try
            {
                #if DEBUG
                Logger.Instance.LogMessage(TracingLevel.Debug, $"Plugin Keydown: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
                #endif

                if (!Instances.TryGetValue(e.Event.Context, out ICommonPluginFunctions instance)) return;
                
                var payload = new KeyPayload(
                    e.Event.Payload?.Coordinates,
                    e.Event.Payload?.Settings,
                    e.Event.Payload?.State,
                    e.Event.Payload?.UserDesiredState ?? 0,
                    e.Event.Payload?.IsInMultiAction ?? false);
                
                if (instance is IKeypadPlugin plugin)
                {
                    plugin.KeyPressed(payload);
                }
                else
                {
                    Logger.Instance.LogMessage(TracingLevel.Error, $"Keydown General Error: Could not convert {e.Event.Context} to IKeypadPlugin");
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        // Button released
        private async void Connection_OnKeyUp(object sender, SdEventReceivedEventArgs<KeyUpEvent> e)
        {
            if (updateHandler?.IsBlockingUpdate ?? false) return;
            
            await instancesLock.WaitAsync();
            try
            {
                #if DEBUG
                Logger.Instance.LogMessage(TracingLevel.Debug, $"Plugin Keyup: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
                #endif

                if (!Instances.TryGetValue(e.Event.Context, out ICommonPluginFunctions instance)) return;
                
                var payload = new KeyPayload(
                    e.Event.Payload?.Coordinates,
                    e.Event.Payload?.Settings,
                    e.Event.Payload?.State,
                    e.Event.Payload?.UserDesiredState ?? 0,
                    e.Event.Payload?.IsInMultiAction ?? false);
                
                if (instance is IKeypadPlugin plugin)
                {
                    plugin.KeyReleased(payload);
                }
                else
                {
                    Logger.Instance.LogMessage(TracingLevel.Error, $"Keyup General Error: Could not convert {e.Event.Context} to IKeypadPlugin");
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        // Function runs every second, used to update UI
        private async void RunTick()
        {
            if (updateHandler?.IsBlockingUpdate ?? false) return;

            await instancesLock.WaitAsync();
            try
            {
                foreach (KeyValuePair<string, ICommonPluginFunctions> kvp in Instances.ToArray())
                {
                    kvp.Value.OnTick();
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        // Action is loaded in the Stream Deck
        private async void Connection_OnWillAppear(object sender, SdEventReceivedEventArgs<WillAppearEvent> e)
        {
            var conn = new SdConnection(connection, pluginUuid, deviceInfo, e.Event.Action, e.Event.Context, e.Event.Device);
            await instancesLock.WaitAsync();
            try
            {
                #if DEBUG
                Logger.Instance.LogMessage(TracingLevel.Debug, $"Plugin OnWillAppear: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
                #endif

                if (actionRegistry.PluginActionIDs().Contains(e.Event.Action))
                {
                    try
                    {
                        if (Instances.TryGetValue(e.Event.Context, out ICommonPluginFunctions instance) && instance != null)
                        {
                            Logger.Instance.LogMessage(TracingLevel.Info, $"WillAppear called for already existing context {e.Event.Context} (might be inside a multi-action)");
                            return;
                        }
                        var payload = new InitialPayload(e.Event.Payload, deviceInfo);
                        Instances[e.Event.Context] = actionRegistry.CreateAction(e.Event.Action, conn, payload);
                        
                        #if DEBUG
                        Logger.Instance.LogMessage(TracingLevel.Debug, $"Instance count is now {Instances.Count}");
                        #endif
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.LogMessage(TracingLevel.Fatal, $"Could not create instance of {e.Event.Action} with context {e.Event.Context} - This may be due to an Exception raised in the constructor, or the class does not inherit PluginBase with the same constructor {ex}");
                    }
                }
                else
                {
                    Logger.Instance.LogMessage(TracingLevel.Warn, $"No plugin found that matches action: {e.Event.Action}");
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        private async void Connection_OnWillDisappear(object sender, SdEventReceivedEventArgs<WillDisappearEvent> e)
        {
            await instancesLock.WaitAsync();
            try
            {
                #if DEBUG
                Logger.Instance.LogMessage(TracingLevel.Debug, $"Plugin OnWillDisappear: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
                #endif

                if (!Instances.TryGetValue(e.Event.Context, out ICommonPluginFunctions value)) return;
                value.Destroy();
                Instances.Remove(e.Event.Context);
                
                #if DEBUG
                Logger.Instance.LogMessage(TracingLevel.Debug, $"Instance count is now {Instances.Count}");
                #endif
            }
            finally
            {
                instancesLock.Release();
            }
        }

        // Settings updated
        private async void Connection_OnDidReceiveSettings(object sender, SdEventReceivedEventArgs<DidReceiveSettingsEvent> e)
        {
            await instancesLock.WaitAsync();
            try
            {
                #if DEBUG
                Logger.Instance.LogMessage(TracingLevel.Debug, $"Plugin OnDidReceiveSettings: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
                #endif

                if (Instances.TryGetValue(e.Event.Context, out ICommonPluginFunctions instance))
                {
                    instance.ReceivedSettings(JObject.FromObject(e.Event.Payload ?? new ReceivedSettingsPayload()).ToObject<ReceivedSettingsPayload>());
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        // Global settings updated
        private async void Connection_OnDidReceiveGlobalSettings(object sender, SdEventReceivedEventArgs<DidReceiveGlobalSettingsEvent> e)
        {
            await instancesLock.WaitAsync();
            try
            {
                #if DEBUG
                Logger.Instance.LogMessage(TracingLevel.Debug, $"Plugin OnDidReceiveGlobalSettings: Settings: {e.Event.Payload?.ToStringEx()}");
                #endif

                var globalSettings = JObject.FromObject(e.Event.Payload ?? new ReceivedGlobalSettingsPayload()).ToObject<ReceivedGlobalSettingsPayload>();
                foreach (string key in Instances.Keys)
                {
                    Instances[key].ReceivedGlobalSettings(globalSettings);
                }

                updateHandler?.SetGlobalSettings(globalSettings);
            }
            finally
            {
                instancesLock.Release();
            }
        }

        private async void Connection_OnTouchpadPress(object sender, SdEventReceivedEventArgs<TouchTapEvent> e)
        {
            if (updateHandler?.IsBlockingUpdate ?? false)
            {
                if (!string.IsNullOrEmpty(lastUpdateInfo?.UpdateUrl))
                {
                    await connection.OpenUrlAsync(lastUpdateInfo.UpdateUrl);
                }
                return;
            }

            await instancesLock.WaitAsync();
            try
            {
                #if DEBUG
                Logger.Instance.LogMessage(TracingLevel.Debug, $"TouchpadPress: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
                #endif

                if (!Instances.TryGetValue(e.Event.Context, out ICommonPluginFunctions instance)) return;
                
                var payload = new TouchpadPressPayload(
                    e.Event.Payload?.Coordinates,
                    e.Event.Payload?.Settings,
                    e.Event.Payload?.Controller,
                    e.Event.Payload?.IsLongPress ?? false,
                    e.Event.Payload?.TapPosition);
                
                if (instance is IEncoderPlugin plugin)
                {
                    plugin.TouchPress(payload);
                }
                else
                {
                    Logger.Instance.LogMessage(TracingLevel.Error, $"TouchpadPress General Error: Could not convert {e.Event.Context} to IEncoderPlugin");
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        // Dial Up

        private async void Connection_OnDialUp(object sender, SdEventReceivedEventArgs<DialUpEvent> e)
        {
            if (updateHandler?.IsBlockingUpdate ?? false) return;
            
            await instancesLock.WaitAsync();
            try
            {
                #if DEBUG
                Logger.Instance.LogMessage(TracingLevel.Debug, $"DialPress: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
                #endif

                if (!Instances.TryGetValue(e.Event.Context, out ICommonPluginFunctions instance)) return;
                
                var payload = new DialPayload(
                    e.Event.Payload?.Coordinates,
                    e.Event.Payload?.Settings,
                    e.Event.Payload?.Controller);
                
                if (instance is IEncoderPlugin plugin)
                {
                    plugin.DialUp(payload);
                }
                else
                {
                    Logger.Instance.LogMessage(TracingLevel.Error, $"DialDown General Error: Could not convert {e.Event.Context} to IEncoderPlugin");
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        // Dial Down
        private async void Connection_OnDialDown(object sender, SdEventReceivedEventArgs<DialDownEvent> e)
        {
            if (updateHandler?.IsBlockingUpdate ?? false)
            {
                if (!string.IsNullOrEmpty(lastUpdateInfo?.UpdateUrl))
                {
                    await connection.OpenUrlAsync(lastUpdateInfo.UpdateUrl);
                }
                return;
            }

            await instancesLock.WaitAsync();
            try
            {
                #if DEBUG
                Logger.Instance.LogMessage(TracingLevel.Debug, $"DialPress: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
                #endif

                if (!Instances.TryGetValue(e.Event.Context, out ICommonPluginFunctions instance)) return;
                
                var payload = new DialPayload(
                    e.Event.Payload?.Coordinates,
                    e.Event.Payload?.Settings,
                    e.Event.Payload?.Controller);
                
                if (instance is IEncoderPlugin plugin)
                {
                    plugin.DialDown(payload);
                }
                else
                {
                    Logger.Instance.LogMessage(TracingLevel.Error, $"DialDown General Error: Could not convert {e.Event.Context} to IEncoderPlugin");
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        private async void Connection_OnDialRotate(object sender, SdEventReceivedEventArgs<DialRotateEvent> e)
        {
            if (updateHandler?.IsBlockingUpdate ?? false) return;
            
            await instancesLock.WaitAsync();
            try
            {
                #if DEBUG
                Logger.Instance.LogMessage(TracingLevel.Debug, $"DialRotate: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
                #endif

                if (!Instances.TryGetValue(e.Event.Context, out ICommonPluginFunctions instance)) return;
                
                var payload = new DialRotatePayload(
                    e.Event.Payload?.Coordinates,
                    e.Event.Payload?.Settings,
                    e.Event.Payload?.Controller,
                    e.Event.Payload?.Ticks ?? 0,
                    e.Event.Payload?.IsDialPressed ?? false);
                
                if (instance is IEncoderPlugin plugin)
                {
                    plugin.DialRotate(payload);
                }
                else
                {
                    Logger.Instance.LogMessage(TracingLevel.Error, $"DialRotate General Error: Could not convert {e.Event.Context} to IEncoderPlugin");
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        private async void UpdateHandler_OnUpdateStatusChanged(object sender, PluginUpdateInfo e)
        {
            lastUpdateInfo = e;
            if (!string.IsNullOrEmpty(e.UpdateUrl))
            {
                await connection.OpenUrlAsync(e.UpdateUrl);
            }

            if (!string.IsNullOrEmpty(e.UpdateImage))
            {
                await Task.Run(async () =>
                {
                    foreach (string contextId in Instances.Keys.ToList())
                    {
                        await connection.SetImageAsync(e.UpdateImage, contextId, SdkTarget.HardwareAndSoftware, null);
                        await connection.SetTitleAsync(null, contextId, SdkTarget.HardwareAndSoftware, null);
                    }
                });
            }

            if (e.Status != PluginUpdateStatus.CriticalUpgrade) return;
            Logger.Instance.LogMessage(TracingLevel.Fatal, $"Critical update needed");
            Environment.Exit(0);
        }

        private void Connection_OnConnected(object sender, EventArgs e)
        {
            connectEvent.Set();
        }

        private void Connection_OnDisconnected(object sender, EventArgs e)
        {
            Logger.Instance.LogMessage(TracingLevel.Info, "Disconnect event received");
            disconnectEvent.Set();
        }
    }
}
