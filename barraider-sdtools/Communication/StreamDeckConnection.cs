using BarRaider.SdTools.Communication.Messages;
using BarRaider.SdTools.Communication.SDEvents;
using BarRaider.SdTools.Wrappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BarRaider.SdTools.Utilities;
using SkiaSharp;

namespace BarRaider.SdTools.Communication
{
    /// <summary>
    /// Underlying object that communicates with the stream deck app
    /// </summary>
    public class StreamDeckConnection
    {
        private const int BUFFER_SIZE = 1024 * 1024;

        private ClientWebSocket webSocket;
        private readonly SemaphoreSlim sendSocketSemaphore = new(1);
        private readonly CancellationTokenSource cancelTokenSource = new();
        private readonly string registerEvent;

        /// <summary>
        /// The port used to connect to the StreamDeck websocket
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// This is the unique identifier used to communicate with the register StreamDeck plugin.
        /// </summary>
        public string Uuid { get; private set; }

        #region Public Events

        /// <summary>
        /// Raised when plugin is connected to stream deck app
        /// </summary>
        public event EventHandler<EventArgs> OnConnected;

        /// <summary>
        /// /// Raised when plugin is disconnected from stream deck app
        /// </summary>
        public event EventHandler<EventArgs> OnDisconnected;

        /// <summary>
        /// Raised when key is pushed down
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<KeyDownEvent>> OnKeyDown;

        /// <summary>
        /// Raised when key is released
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<KeyUpEvent>> OnKeyUp;

        /// <summary>
        /// Raised when the action is shown, main trigger for a PluginAction
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<WillAppearEvent>> OnWillAppear;

        /// <summary>
        /// Raised when the action is no longer shown, main trigger for Dispose of PluginAction
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<WillDisappearEvent>> OnWillDisappear;

        /// <summary>
        /// Contains information on the Title and its style
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<TitleParametersDidChangeEvent>> OnTitleParametersDidChange;

        /// <summary>
        /// Raised when a Stream Deck device is connected to the PC
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<DeviceDidConnectEvent>> OnDeviceDidConnect;

        /// <summary>
        /// Raised when a Stream Deck device has disconnected from the PC
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<DeviceDidDisconnectEvent>> OnDeviceDidDisconnect;

        /// <summary>
        /// Raised when a monitored app is launched/active
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<ApplicationDidLaunchEvent>> OnApplicationDidLaunch;

        /// <summary>
        /// Raised when a monitored app is terminated
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<ApplicationDidTerminateEvent>> OnApplicationDidTerminate;

        /// <summary>
        /// Raised after the PC wakes up from sleep
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<SystemDidWakeUpEvent>> OnSystemDidWakeUp;

        /// <summary>
        /// Raised when settings for the action are received
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<DidReceiveSettingsEvent>> OnDidReceiveSettings;

        /// <summary>
        /// Raised when global settings for the entire plugin are received
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<DidReceiveGlobalSettingsEvent>> OnDidReceiveGlobalSettings;

        /// <summary>
        /// Raised when the user is viewing the settings in the Stream Deck app
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<PropertyInspectorDidAppearEvent>> OnPropertyInspectorDidAppear;

        /// <summary>
        /// Raised when the user stops viewing the settings in the Stream Deck app
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<PropertyInspectorDidDisappearEvent>> OnPropertyInspectorDidDisappear;

        /// <summary>
        /// Raised when a payload is sent to the plugin from the PI
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<SendToPluginEvent>> OnSendToPlugin;

        /// <summary>
        /// Raised when a dial is rotated
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<DialRotateEvent>> OnDialRotate;

        /// <summary>
        /// Raised when a dial is down
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<DialDownEvent>> OnDialDown;

        /// <summary>
        /// Raised when a dial is up
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<DialUpEvent>> OnDialUp;

        /// <summary>
        /// Raised when the touchpad is pressed
        /// </summary>
        public event EventHandler<SdEventReceivedEventArgs<TouchTapEvent>> OnTouchpadPress;

        #endregion

        internal StreamDeckConnection(int port, string uuid, string registerEvent)
        {
            Port = port;
            Uuid = uuid;
            this.registerEvent = registerEvent;
        }

        internal void Run()
        {
            if (webSocket != null) return;
            webSocket = new ClientWebSocket();
            _ = RunAsync();
        }

        internal void Stop()
        {
            cancelTokenSource.Cancel();
        }

        internal Task SendAsync(IMessage message)
        {
            try
            {
                return SendAsync(JsonConvert.SerializeObject(message));
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.Error, $"{GetType()} SDTools SendAsync Exception: {ex}");
            }
            return null;
        }

        #region Requests

        internal Task SetTitleAsync(string title, string context, SdkTarget target, int? state)
        {
            return SendAsync(new SetTitleMessage(title, context, target, state));
        }

        internal Task LogMessageAsync(string message)
        {
            return SendAsync(new LogMessage(message));
        }

        internal Task SetImageAsync(SKData data, string context, SdkTarget target, int? state)
        {
            try
            {
                byte[] imageBytes = data.ToArray();

                // Convert byte[] to Base64 String
                var base64String = $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
                return SetImageAsync(base64String, context, target, state);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.Error, $"{GetType()} SetImageAsync Exception: {ex}");
            }
            return null;
        }

        internal Task SetImageAsync(string base64Image, string context, SdkTarget target, int? state)
        {
            return SendAsync(new SetImageMessage(base64Image, context, target, state));
        }

        internal Task ShowAlertAsync(string context)
        {
            return SendAsync(new ShowAlertMessage(context));
        }

        internal Task ShowOkAsync(string context)
        {
            return SendAsync(new ShowOkMessage(context));
        }

        internal Task SetGlobalSettingsAsync(JObject settings)
        {
            return SendAsync(new SetGlobalSettingsMessage(settings, Uuid));
        }

        internal Task GetGlobalSettingsAsync()
        {
            return SendAsync(new GetGlobalSettingsMessage(Uuid));
        }

        internal Task SetSettingsAsync(JObject settings, string context)
        {
            return SendAsync(new SetSettingsMessage(settings, context));
        }

        internal Task GetSettingsAsync(string context)
        {
            return SendAsync(new GetSettingsMessage(context));
        }

        internal Task SetStateAsync(uint? state, string context)
        {
            return SendAsync(new SetStateMessage(state, context));
        }

        internal Task SendToPropertyInspectorAsync(string action, JObject data, string context)
        {
            return SendAsync(new SendToPropertyInspectorMessage(action, data, context));
        }

        internal Task SwitchToProfileAsync(string device, string profileName, string context)
        {
            return SendAsync(new SwitchToProfileMessage(device, profileName, context));
        }
        internal Task OpenUrlAsync(string uri)
        {
            return OpenUrlAsync(new Uri(uri));
        }

        internal Task OpenUrlAsync(Uri uri)
        {
            return SendAsync(new OpenUrlMessage(uri));
        }

        internal Task SetFeedbackAsync(Dictionary<string, string> dictKeyValues, string context)
        {
            return SendAsync(new SetFeedbackMessage(dictKeyValues, context));
        }

        internal Task SetFeedbackAsync(JObject feedbackPayload, string context)
        {
            return SendAsync(new SetFeedbackMessageEx(feedbackPayload, context));
        }

        internal Task SetFeedbackLayoutAsync(string layout, string context)
        {
            return SendAsync(new SetFeedbackLayoutMessage(layout, context));
        }
        #endregion

        #region Private Methods
        private async Task SendAsync(string text)
        {
            try
            {
                if (webSocket != null)
                {
                    try
                    {
                        await sendSocketSemaphore.WaitAsync();
                        byte[] buffer = Encoding.UTF8.GetBytes(text);
                        await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancelTokenSource.Token);
                    }
                    finally
                    {
                        sendSocketSemaphore.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.Fatal, $"{GetType()} SendAsync Exception: {ex}");
                await DisconnectAsync();
            }

        }

        private async Task RunAsync()
        {
            try
            {
                await webSocket.ConnectAsync(new Uri($"ws://localhost:{Port}"), cancelTokenSource.Token);
                if (webSocket.State != WebSocketState.Open)
                {

                    Logger.Instance.LogMessage(TracingLevel.Fatal, $"{GetType()} RunAsync failed - Websocket not open {webSocket.State}");
                    await DisconnectAsync();
                    return;
                }

                await SendAsync(new RegisterEventMessage(registerEvent, Uuid));

                OnConnected?.Invoke(this, new EventArgs());
                await ReceiveAsync();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.Fatal, $"{GetType()} ReceiveAsync Exception: {ex}");
            }
            finally
            {
                Logger.Instance.LogMessage(TracingLevel.Info, $"{GetType()} RunAsync completed, shutting down");
                await DisconnectAsync();
            }
        }

        private async Task<WebSocketCloseStatus> ReceiveAsync()
        {
            var buffer = new byte[BUFFER_SIZE];
            var arrayBuffer = new ArraySegment<byte>(buffer);
            var textBuffer = new StringBuilder(BUFFER_SIZE);

            try
            {
                while (!cancelTokenSource.IsCancellationRequested && webSocket != null)
                {
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(arrayBuffer, cancelTokenSource.Token);

                    if (result == null) continue;
                    
                    if (result.MessageType == WebSocketMessageType.Close ||
                        (result.CloseStatus is not null && result.CloseStatus.Value != WebSocketCloseStatus.Empty))
                    {
                        string closeStatus = (result.CloseStatus == null) ? "None" : (result.CloseStatus.HasValue) ? result.CloseStatus.Value.ToString() : "None";
                        
                        Logger.Instance.LogMessage(TracingLevel.Info, $"{GetType()} Received websocket close message. MessageType: {result.MessageType} CloseStatus: {closeStatus}");
                        return result.CloseStatus.GetValueOrDefault();
                    }

                    if (result.MessageType != WebSocketMessageType.Text) continue;
                    textBuffer.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    if (!result.EndOfMessage) continue;
                        
                    #if DEBUG
                    Logger.Instance.LogMessage(TracingLevel.Debug, $"Incoming Message: {textBuffer}");
                    #endif

                    var strBuffer = textBuffer.ToString();
                    textBuffer.Clear();
                    BaseEvent evt = BaseEvent.Parse(strBuffer);
                    if (evt == null)
                    {
                        Logger.Instance.LogMessage(TracingLevel.Warn, $"{GetType()} Unknown event received from Stream Deck: {strBuffer}");
                        continue;
                    }

                    try
                    {
                        switch (evt.Event)
                        {
                            case EventTypes.KEY_DOWN: OnKeyDown?.Invoke(this, new SdEventReceivedEventArgs<KeyDownEvent>(evt as KeyDownEvent)); break;
                            case EventTypes.KEY_UP: OnKeyUp?.Invoke(this, new SdEventReceivedEventArgs<KeyUpEvent>(evt as KeyUpEvent)); break;
                            case EventTypes.WILL_APPEAR: OnWillAppear?.Invoke(this, new SdEventReceivedEventArgs<WillAppearEvent>(evt as WillAppearEvent)); break;
                            case EventTypes.WILL_DISAPPEAR: OnWillDisappear?.Invoke(this, new SdEventReceivedEventArgs<WillDisappearEvent>(evt as WillDisappearEvent)); break;
                            case EventTypes.TITLE_PARAMETERS_DID_CHANGE: OnTitleParametersDidChange?.Invoke(this, new SdEventReceivedEventArgs<TitleParametersDidChangeEvent>(evt as TitleParametersDidChangeEvent)); break;
                            case EventTypes.DEVICE_DID_CONNECT: OnDeviceDidConnect?.Invoke(this, new SdEventReceivedEventArgs<DeviceDidConnectEvent>(evt as DeviceDidConnectEvent)); break;
                            case EventTypes.DEVICE_DID_DISCONNECT: OnDeviceDidDisconnect?.Invoke(this, new SdEventReceivedEventArgs<DeviceDidDisconnectEvent>(evt as DeviceDidDisconnectEvent)); break;
                            case EventTypes.APPLICATION_DID_LAUNCH: OnApplicationDidLaunch?.Invoke(this, new SdEventReceivedEventArgs<ApplicationDidLaunchEvent>(evt as ApplicationDidLaunchEvent)); break;
                            case EventTypes.APPLICATION_DID_TERMINATE: OnApplicationDidTerminate?.Invoke(this, new SdEventReceivedEventArgs<ApplicationDidTerminateEvent>(evt as ApplicationDidTerminateEvent)); break;
                            case EventTypes.SYSTEM_DID_WAKE_UP: OnSystemDidWakeUp?.Invoke(this, new SdEventReceivedEventArgs<SystemDidWakeUpEvent>(evt as SystemDidWakeUpEvent)); break;
                            case EventTypes.DID_RECEIVE_SETTINGS: OnDidReceiveSettings?.Invoke(this, new SdEventReceivedEventArgs<DidReceiveSettingsEvent>(evt as DidReceiveSettingsEvent)); break;
                            case EventTypes.DID_RECEIVE_GLOBAL_SETTINGS: OnDidReceiveGlobalSettings?.Invoke(this, new SdEventReceivedEventArgs<DidReceiveGlobalSettingsEvent>(evt as DidReceiveGlobalSettingsEvent)); break;
                            case EventTypes.PROPERTY_INSPECTOR_DID_APPEAR: OnPropertyInspectorDidAppear?.Invoke(this, new SdEventReceivedEventArgs<PropertyInspectorDidAppearEvent>(evt as PropertyInspectorDidAppearEvent)); break;
                            case EventTypes.PROPERTY_INSPECTOR_DID_DISAPPEAR: OnPropertyInspectorDidDisappear?.Invoke(this, new SdEventReceivedEventArgs<PropertyInspectorDidDisappearEvent>(evt as PropertyInspectorDidDisappearEvent)); break;
                            case EventTypes.SEND_TO_PLUGIN: OnSendToPlugin?.Invoke(this, new SdEventReceivedEventArgs<SendToPluginEvent>(evt as SendToPluginEvent)); break;
                            case EventTypes.DIAL_ROTATE: OnDialRotate?.Invoke(this, new SdEventReceivedEventArgs<DialRotateEvent>(evt as DialRotateEvent)); break;
                            case EventTypes.DIAL_DOWN: OnDialDown?.Invoke(this, new SdEventReceivedEventArgs<DialDownEvent>(evt as DialDownEvent)); break;
                            case EventTypes.DIAL_UP: OnDialUp?.Invoke(this, new SdEventReceivedEventArgs<DialUpEvent>(evt as DialUpEvent)); break;
                            case EventTypes.TOUCHTAP: OnTouchpadPress?.Invoke(this, new SdEventReceivedEventArgs<TouchTapEvent>(evt as TouchTapEvent)); break;
                            default:
                                Logger.Instance.LogMessage(TracingLevel.Warn, $"{GetType()} Unsupported Stream Deck event: {strBuffer}");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.LogMessage(TracingLevel.Error, $"{GetType()} Unhandled 3rd party exception when triggering {evt.Event} event. Exception: {ex}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.Fatal, $"{GetType()} ReceiveAsync Exception: {ex}");
            }

            Logger.Instance.LogMessage(TracingLevel.Info, $"{GetType()} ReceiveAsync ended with CancelToken: {cancelTokenSource.IsCancellationRequested} Websocket: {(webSocket == null ? "null" : "valid")}");
            return WebSocketCloseStatus.NormalClosure;
        }

        private async Task DisconnectAsync()
        {
            if (webSocket != null)
            {
                ClientWebSocket socket = webSocket;
                webSocket = null;

                try
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnecting", cancelTokenSource.Token);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogMessage(TracingLevel.Error, $"{GetType()} DisconnectAsync failed to close connection. Exception: {ex}");
                }


                try
                {
                    socket.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogMessage(TracingLevel.Error, $"{GetType()} DisconnectAsync failed to dispose websocket. Exception: {ex}");
                }

                OnDisconnected?.Invoke(this, EventArgs.Empty);
            }
        }
        #endregion
    }
}
