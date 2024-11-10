using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BarRaider.SdTools.Communication;
using BarRaider.SdTools.Communication.SDEvents;
using BarRaider.SdTools.Events;
using BarRaider.SdTools.Payloads;
using BarRaider.SdTools.StreamDeckInfo;
using BarRaider.SdTools.Utilities;
using BarRaider.SdTools.Wrappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SkiaSharp;

namespace BarRaider.SdTools.Backend
{
    /// <summary>
    /// Connection object which handles your communication with the Stream Deck app
    /// </summary>
    public class SdConnection : ISdConnection
    {
        #region Private Members
        private string previousImageHash;

        [JsonIgnore]
        private readonly string actionId;

        /// <summary>
        /// An opaque value identifying the plugin. Received as an argument when the executable was launched.
        /// </summary>
        [JsonIgnore]
        private readonly string pluginUuid;

        /// <summary>
        /// Holds information about the devices connected to the computer
        /// </summary>
        [JsonIgnore]
        private readonly RegistrationInfo deviceInfo;
        #endregion

        #region Public Events
        /// <summary>
        /// Event received by the plugin when the Property Inspector uses the sendToPlugin event.
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<SendToPlugin>> OnSendToPlugin;
        
        /// <summary>
        /// Event received when the user changes the title or title parameters.
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<TitleParametersDidChange>> OnTitleParametersDidChange;
        
        /// <summary>
        /// Event received when a monitored application is terminated
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<ApplicationDidTerminate>> OnApplicationDidTerminate;
        
        /// <summary>
        /// Event received when a monitored application is launched
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<ApplicationDidLaunch>> OnApplicationDidLaunch;
        
        /// <summary>
        /// Event received when a device is unplugged from the computer
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<DeviceDidDisconnect>> OnDeviceDidDisconnect;
        
        /// <summary>
        /// Event received when a device is plugged to the computer.
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<DeviceDidConnect>> OnDeviceDidConnect;
        
        /// <summary>
        /// Event received when the Property Inspector appears in the Stream Deck software user interface, for example when selecting a new instance.
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<PropertyInspectorDidAppear>> OnPropertyInspectorDidAppear;
        
        /// <summary>
        /// Event received when the Property Inspector for an instance is removed from the Stream Deck software user interface, for example when selecting a different instance.
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<PropertyInspectorDidDisappear>> OnPropertyInspectorDidDisappear;
        
        /// <summary>
        /// Event received when the computer wakes up
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<SystemDidWakeUp>> OnSystemDidWakeUp;
        #endregion

        #region Public Properties
        /// <summary>
        /// An opaque value identifying the plugin. This value is received during the Registration procedure
        /// </summary>
        [JsonIgnore]
        public string ContextId { get; private set; }

        /// <summary>
        /// An opaque value identifying the device the plugin is launched on.
        /// </summary>
        [JsonIgnore]
        public string DeviceId { get; private set; }

        /// <summary>
        /// StreamDeckConnection object, initialized based on the args received when launching the program
        /// </summary>
        [JsonIgnore]
        public StreamDeckConnection StreamDeckConnection { get; private set; }

        #endregion

        /// <summary>
        /// Public constructor, a StreamDeckConnection object is required along with the current action and context IDs
        /// These will be used to correctly communicate with the StreamDeck App
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pluginUuid"></param>
        /// <param name="deviceInfo"></param>
        /// <param name="actionId"></param>
        /// <param name="contextId"></param>
        /// /// <param name="deviceId"></param>
        public SdConnection(
            StreamDeckConnection connection,
            string pluginUuid,
            RegistrationInfo deviceInfo,
            string actionId,
            string contextId,
            string deviceId)
        {
            StreamDeckConnection = connection;
            this.pluginUuid = pluginUuid;
            this.deviceInfo = deviceInfo;
            this.actionId = actionId;
            ContextId = contextId;
            DeviceId = deviceId;

            StreamDeckConnection.OnSendToPlugin += Connection_OnSendToPlugin;
            StreamDeckConnection.OnTitleParametersDidChange += Connection_OnTitleParametersDidChange;
            StreamDeckConnection.OnApplicationDidTerminate += Connection_OnApplicationDidTerminate;
            StreamDeckConnection.OnApplicationDidLaunch += Connection_OnApplicationDidLaunch;
            StreamDeckConnection.OnDeviceDidDisconnect += Connection_OnDeviceDidDisconnect;
            StreamDeckConnection.OnDeviceDidConnect += Connection_OnDeviceDidConnect;
            StreamDeckConnection.OnPropertyInspectorDidAppear += Connection_OnPropertyInspectorDidAppear;
            StreamDeckConnection.OnPropertyInspectorDidDisappear += Connection_OnPropertyInspectorDidDisappear;
            StreamDeckConnection.OnSystemDidWakeUp += StreamDeckConnection_OnSystemDidWakeUp;
        }

        #region Public Methods
        /// <summary>
        /// Dispose (Destructor) function
        /// </summary>
        public void Dispose()
        {
            StreamDeckConnection.OnSendToPlugin -= Connection_OnSendToPlugin;
            StreamDeckConnection.OnTitleParametersDidChange -= Connection_OnTitleParametersDidChange;
            StreamDeckConnection.OnApplicationDidTerminate -= Connection_OnApplicationDidTerminate;
            StreamDeckConnection.OnApplicationDidLaunch -= Connection_OnApplicationDidLaunch;
            StreamDeckConnection.OnDeviceDidDisconnect -= Connection_OnDeviceDidDisconnect;
            StreamDeckConnection.OnDeviceDidConnect -= Connection_OnDeviceDidConnect;
            StreamDeckConnection.OnPropertyInspectorDidAppear -= Connection_OnPropertyInspectorDidAppear;
            StreamDeckConnection.OnPropertyInspectorDidDisappear -= Connection_OnPropertyInspectorDidDisappear;
            StreamDeckConnection.OnSystemDidWakeUp -= StreamDeckConnection_OnSystemDidWakeUp;
        }

        /// <summary>
        /// Gets the Stream Deck device's info
        /// </summary>
        /// <returns></returns>
        public StreamDeckDeviceInfo DeviceInfo()
        {
            if (deviceInfo != null && !string.IsNullOrEmpty(DeviceId))
            {
                return deviceInfo.Devices.FirstOrDefault(d => d.Id == DeviceId);
            }
            
            Logger.Instance.LogMessage(TracingLevel.Error, $"Could not get DeviceInfo for DeviceId: {DeviceId} Devices: {deviceInfo?.Devices?.Length}");
            return null;
        }
        #endregion

        #region Public Requests
        /// <summary>
        /// Send settings to the PropertyInspector
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public async Task SendToPropertyInspectorAsync(JObject settings)
        {
            if (StreamDeckConnection != null && !string.IsNullOrEmpty(ContextId) && !string.IsNullOrEmpty(actionId))
            {
                await StreamDeckConnection.SendToPropertyInspectorAsync(actionId, settings, ContextId);
            }
        }

        /// <summary>
        /// Persists your plugin settings
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public async Task SetSettingsAsync(JObject settings)
        {
            if (StreamDeckConnection != null && !string.IsNullOrEmpty(ContextId) && !string.IsNullOrEmpty(actionId))
            {
                await StreamDeckConnection.SetSettingsAsync(settings, ContextId);
            }
        }

        /// <summary>
        /// Persists your global plugin settings
        /// </summary>
        /// <param name="settings">Settings to save globally</param>
        /// <param name="triggerDidReceiveGlobalSettings">Boolean whether to also trigger a didReceiveGlobalSettings event. Default is true</param>
        /// <returns></returns>
        public async Task SetGlobalSettingsAsync(JObject settings, bool triggerDidReceiveGlobalSettings = true)
        {
            if (StreamDeckConnection != null)
            {
                await StreamDeckConnection.SetGlobalSettingsAsync(settings);

                if (triggerDidReceiveGlobalSettings)
                {
                    await GetGlobalSettingsAsync();
                }
            }
        }

        /// <summary>
        /// Persists your global plugin settings
        /// </summary>
        /// <returns></returns>
        public async Task GetGlobalSettingsAsync()
        {
            if (StreamDeckConnection != null)
            {
                await StreamDeckConnection.GetGlobalSettingsAsync();
            }
        }

        /// <summary>
        /// Sets an image on the StreamDeck key.
        /// </summary>
        /// <param name="base64Image">Base64 encoded image</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the title is set to all states.</param>
        /// <param name="forceSendToStreamdeck">Should image be sent even if it is identical to the one sent previously. Default is false</param>
        /// <returns></returns>
        public async Task SetImageAsync(string base64Image, int? state = null, bool forceSendToStreamdeck = false)
        {
            string hash = Tools.StringToSha512(base64Image);
            if (forceSendToStreamdeck || hash != previousImageHash)
            {
                previousImageHash = hash;
                await StreamDeckConnection.SetImageAsync(base64Image, ContextId, SdkTarget.HardwareAndSoftware, state);
            }
        }

        /// <summary>
        /// Sets an image on the StreamDeck key
        /// </summary>
        /// <param name="data">An SKData object representing the encoded image</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the title is set to all states.</param>
        /// <param name="forceSendToStreamdeck">Should image be sent even if it is identical to the one sent previously. Default is false</param>
        /// <returns></returns>
        public async Task SetImageAsync(SKData data, int? state = null, bool forceSendToStreamdeck = false)
        {
            string hash = Tools.ImageToSha512(data);
            if (forceSendToStreamdeck || hash != previousImageHash)
            {
                Logger.Instance.LogMessage(TracingLevel.Info, "sending to streamdeck");
                previousImageHash = hash;
                await StreamDeckConnection.SetImageAsync(data, ContextId, SdkTarget.HardwareAndSoftware, state);
            }
        }

        /// <summary>
        /// Sets the default image for this state, as configured in the manifest
        /// </summary>
        /// <returns></returns>
        public async Task SetDefaultImageAsync()
        {
            await SetImageAsync((string)null);
        }

        /// <summary>
        /// Sets a title on the StreamDeck key
        /// </summary>
        /// <param name="title"></param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the title is set to all states.</param>
        /// <returns></returns>
        public async Task SetTitleAsync(string title, int? state = null)
        {
            await StreamDeckConnection.SetTitleAsync(title, ContextId, SdkTarget.HardwareAndSoftware, state);
        }

        /// <summary>
        /// Switches to one of the plugin's built-in profiles
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public async Task SwitchProfileAsync(string profileName)
        {
            await StreamDeckConnection.SwitchToProfileAsync(DeviceId, profileName, pluginUuid);
        }

        /// <summary>
        /// Switches to one of the plugin's built-in profiles. Allows to choose which device to switch it on.
        /// </summary>
        /// <param name="profileName"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task SwitchProfileAsync(string profileName, string deviceId)
        {
            await StreamDeckConnection.SwitchToProfileAsync(deviceId, profileName, pluginUuid);
        }

        /// <summary>
        /// Shows the Alert (Yellow Triangle) on the StreamDeck key
        /// </summary>
        /// <returns></returns>
        public async Task ShowAlert()
        {
            await StreamDeckConnection.ShowAlertAsync(ContextId);
        }

        /// <summary>
        /// Shows the Success (Green checkmark) on the StreamDeck key
        /// </summary>
        /// <returns></returns>
        public async Task ShowOk()
        {
            await StreamDeckConnection.ShowOkAsync(ContextId);
        }

        /// <summary>
        /// Add a message to the Stream Deck log. This is the log located at: %appdata%\Elgato\StreamDeck\logs\StreamDeck0.log
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task LogSdMessage(string message)
        {
            await StreamDeckConnection.LogMessageAsync(message);
        }

        /// <summary>
        /// Tells Stream Deck to return the current plugin settings via the ReceivedSettings function
        /// </summary>
        /// <returns></returns>
        public async Task GetSettingsAsync()
        {
            await StreamDeckConnection.GetSettingsAsync(ContextId);
        }

        /// <summary>
        /// Opens a URI in the user's browser
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task OpenUrlAsync(string uri)
        {
            await StreamDeckConnection.OpenUrlAsync(uri);
        }

        /// <summary>
        /// Opens a URI in the user's browser
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task OpenUrlAsync(Uri uri)
        {
            await StreamDeckConnection.OpenUrlAsync(uri);
        }

        /// <summary>
        /// Sets the plugin to a specific state which is pre-configured in the manifest file
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task SetStateAsync(uint? state)
        {
            await StreamDeckConnection.SetStateAsync(state, ContextId);
        }

        /// <summary>
        /// Sets the values of touchpad layouts items
        /// </summary>
        /// <param name="dictKeyValues"></param>
        /// <returns></returns>
        public async Task SetFeedbackAsync(Dictionary<string, string> dictKeyValues)
        {
            await StreamDeckConnection.SetFeedbackAsync(dictKeyValues, ContextId);
        }

        /// <summary>
        /// Sets the value of a single touchpad layout item
        /// </summary>
        /// <returns></returns>
        public async Task SetFeedbackAsync(string layoutItemKey, string value)
        {
            await StreamDeckConnection.SetFeedbackAsync(new Dictionary<string, string>() { { layoutItemKey, value } }, ContextId);
        }

        /// <summary>
        /// Sets the value of a single touchpad layout item
        /// </summary>
        /// <returns></returns>
        public async Task SetFeedbackAsync(JObject feedbackPayload)
        {
            await StreamDeckConnection.SetFeedbackAsync(feedbackPayload, ContextId);
        }


        /// <summary>
        /// Changes the current Stream Deck+ touch display layout
        /// </summary>
        /// <returns></returns>
        public async Task SetFeedbackLayoutAsync(string layout)
        {
            await StreamDeckConnection.SetFeedbackLayoutAsync(layout, ContextId);
        }
        #endregion

        #region Event Wrappers
        private void Connection_OnPropertyInspectorDidDisappear(object sender, SdEventReceivedEventArgs<PropertyInspectorDidDisappearEvent> e)
        {
            if (e.Event.Context == ContextId)
            {
                OnPropertyInspectorDidDisappear?.Invoke(this, new SdEventReceivedEventArgs<PropertyInspectorDidDisappear>(new PropertyInspectorDidDisappear(e.Event.Action, e.Event.Context, e.Event.Device)));
            }
        }

        private void Connection_OnPropertyInspectorDidAppear(object sender, SdEventReceivedEventArgs<PropertyInspectorDidAppearEvent> e)
        {
            if (e.Event.Context == ContextId)
            {
                OnPropertyInspectorDidAppear?.Invoke(this, new SdEventReceivedEventArgs<PropertyInspectorDidAppear>(new PropertyInspectorDidAppear(e.Event.Action, e.Event.Context, e.Event.Device)));
            }
        }

        private void Connection_OnDeviceDidConnect(object sender, SdEventReceivedEventArgs<DeviceDidConnectEvent> e)
        {
            OnDeviceDidConnect?.Invoke(this, new SdEventReceivedEventArgs<DeviceDidConnect>(new DeviceDidConnect(e.Event.DeviceInfo)));
        }

        private void Connection_OnDeviceDidDisconnect(object sender, SdEventReceivedEventArgs<DeviceDidDisconnectEvent> e)
        {
            OnDeviceDidDisconnect?.Invoke(this, new SdEventReceivedEventArgs<DeviceDidDisconnect>(new DeviceDidDisconnect(e.Event.Device)));
        }

        private void Connection_OnApplicationDidTerminate(object sender, SdEventReceivedEventArgs<ApplicationDidTerminateEvent> e)
        {
            OnApplicationDidTerminate?.Invoke(this, new SdEventReceivedEventArgs<ApplicationDidTerminate>(new ApplicationDidTerminate(new ApplicationPayload(e.Event.Payload.Application))));
        }

        private void Connection_OnApplicationDidLaunch(object sender, SdEventReceivedEventArgs<ApplicationDidLaunchEvent> e)
        {
            OnApplicationDidLaunch?.Invoke(this, new SdEventReceivedEventArgs<ApplicationDidLaunch>(new ApplicationDidLaunch(new ApplicationPayload(e.Event.Payload.Application))));
        }

        private void Connection_OnTitleParametersDidChange(object sender, SdEventReceivedEventArgs<TitleParametersDidChangeEvent> e)
        {
            if (e.Event.Context != ContextId) return;
            
            // Special case to take into account that TitleParameters arrives right after an OnWillAppear
            if (OnTitleParametersDidChange == null)
            {
                if (sender != this)
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(1000);
                        Connection_OnTitleParametersDidChange(this, e);
                    });
                }
                return;
            }

            TitleParametersPayload payload = e.Event.Payload;
            var newPayload = new TitleParametersPayload(payload.Settings, payload.Coordinates, payload.State, payload.Title, payload.TitleParameters);
            OnTitleParametersDidChange?.Invoke(this, new SdEventReceivedEventArgs<TitleParametersDidChange>(new TitleParametersDidChange(e.Event.Action, e.Event.Context, e.Event.Device, newPayload)));
        }

        private void Connection_OnSendToPlugin(object sender, SdEventReceivedEventArgs<SendToPluginEvent> e)
        {
            if (e.Event.Context == ContextId)
            {
                OnSendToPlugin?.Invoke(this, new SdEventReceivedEventArgs<SendToPlugin>(new SendToPlugin(e.Event.Action, e.Event.Context, e.Event.Payload)));
            }
        }

        private void StreamDeckConnection_OnSystemDidWakeUp(object sender, SdEventReceivedEventArgs<SystemDidWakeUpEvent> e)
        {
            OnSystemDidWakeUp?.Invoke(this, new SdEventReceivedEventArgs<SystemDidWakeUp>(new SystemDidWakeUp()));
        }
        #endregion
    }
}
