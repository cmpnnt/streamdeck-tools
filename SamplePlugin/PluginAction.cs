using BarRaider.SdTools.Wrappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using BarRaider.SdTools.Attributes;
using BarRaider.SdTools.Backend;
using BarRaider.SdTools.Payloads;
using BarRaider.SdTools.Utilities;
using SkiaSharp;

namespace SamplePlugin
{
    [PluginActionId("com.test.sdtools.sampleplugin")]
    public class PluginAction : KeyAndEncoderBase
    {
        // TODO: Can the framework be refactored to have a standardized settings class?
        //   See: https://docs.elgato.com/streamdeck/sdk/guides/settings
        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                var instance = new PluginSettings
                {
                    OutputFileName = string.Empty,
                    InputString = string.Empty
                };
                return instance;
            }

            [FilenameProperty]
            [JsonProperty(PropertyName = "outputFileName")]
            public string OutputFileName { get; set; }

            [JsonProperty(PropertyName = "inputString")]
            public string InputString { get; set; }
        }

        #region Private Members
        private readonly PluginSettings settings;
        #endregion
        
        public PluginAction(ISdConnection connection, InitialPayload payload) : base(connection, payload)
        {
            settings = payload.Settings == null || payload.Settings.Count == 0 ?
                PluginSettings.CreateDefaultSettings() :
                payload.Settings.ToObject<PluginSettings>();

            Connection.OnApplicationDidLaunch += Connection_OnApplicationDidLaunch;
            Connection.OnApplicationDidTerminate += Connection_OnApplicationDidTerminate;
            Connection.OnDeviceDidConnect += Connection_OnDeviceDidConnect;
            Connection.OnDeviceDidDisconnect += Connection_OnDeviceDidDisconnect;
            Connection.OnPropertyInspectorDidAppear += Connection_OnPropertyInspectorDidAppear;
            Connection.OnPropertyInspectorDidDisappear += Connection_OnPropertyInspectorDidDisappear;
            Connection.OnSendToPlugin += Connection_OnSendToPlugin;
            Connection.OnTitleParametersDidChange += Connection_OnTitleParametersDidChange;
        }

        private void Connection_OnTitleParametersDidChange(object sender, SdEventReceivedEventArgs<BarRaider.SdTools.Events.TitleParametersDidChange> e)
        {
            // Your logic here. Feel free to remove this and the related event de/registrations if it's not needed
        }

        private void Connection_OnSendToPlugin(object sender, SdEventReceivedEventArgs<BarRaider.SdTools.Events.SendToPlugin> e)
        {
            // Your logic here. Feel free to remove this and the related event de/registrations if it's not needed
        }

        private void Connection_OnPropertyInspectorDidDisappear(object sender, SdEventReceivedEventArgs<BarRaider.SdTools.Events.PropertyInspectorDidDisappear> e)
        {
            // Your logic here. Feel free to remove this and the related event de/registrations if it's not needed
        }

        private void Connection_OnPropertyInspectorDidAppear(object sender, SdEventReceivedEventArgs<BarRaider.SdTools.Events.PropertyInspectorDidAppear> e)
        {
            // Your logic here. Feel free to remove this and the related event de/registrations if it's not needed
        }

        private void Connection_OnDeviceDidDisconnect(object sender, SdEventReceivedEventArgs<BarRaider.SdTools.Events.DeviceDidDisconnect> e)
        {
            // Your logic here. Feel free to remove this and the related event de/registrations if it's not needed
        }

        private void Connection_OnDeviceDidConnect(object sender, SdEventReceivedEventArgs<BarRaider.SdTools.Events.DeviceDidConnect> e)
        {
            // Your logic here. Feel free to remove this and the related event de/registrations if it's not needed
        }

        private void Connection_OnApplicationDidTerminate(object sender, SdEventReceivedEventArgs<BarRaider.SdTools.Events.ApplicationDidTerminate> e)
        {
            // Your logic here. Feel free to remove this and the related event de/registrations if it's not needed
        }

        private void Connection_OnApplicationDidLaunch(object sender, SdEventReceivedEventArgs<BarRaider.SdTools.Events.ApplicationDidLaunch> e)
        {
            // Your logic here. Feel free to remove this and the related event de/registrations if it's not needed
        }

        public override void Dispose()
        {
            Connection.OnApplicationDidLaunch -= Connection_OnApplicationDidLaunch;
            Connection.OnApplicationDidTerminate -= Connection_OnApplicationDidTerminate;
            Connection.OnDeviceDidConnect -= Connection_OnDeviceDidConnect;
            Connection.OnDeviceDidDisconnect -= Connection_OnDeviceDidDisconnect;
            Connection.OnPropertyInspectorDidAppear -= Connection_OnPropertyInspectorDidAppear;
            Connection.OnPropertyInspectorDidDisappear -= Connection_OnPropertyInspectorDidDisappear;
            Connection.OnSendToPlugin -= Connection_OnSendToPlugin;
            Connection.OnTitleParametersDidChange -= Connection_OnTitleParametersDidChange;
            Logger.Instance.LogMessage(TracingLevel.Info, $"Destructor called");
        }

        public override void DialRotate(DialRotatePayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.Info, "Dial rotated");
        }

        public override void DialDown(DialPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.Info, "Dial pressed");
        }

        public override void DialUp(DialPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.Info, "Dial released");
        }

        public override void TouchPress(TouchpadPressPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.Info, "Touchpad pressed");
        }

        public override async void KeyPressed(KeyPayload payload)
        {
            // Just some example busy work to do when the button is released
            var tp = new TitleParameters()
            {
                FontFamily = SKTypeface.FromFamilyName("Arial"),
                FontStyle = SKFontStyle.Bold,
                FontSizeInPoints = 9f,
                TitleColor = SKColors.Gray,
            };
            
            using (SKData data = Tools.GenerateKeyImage(tp, "Test", SKColors.White))
            {
                await Connection.SetImageAsync(data);
            }
            
            Logger.Instance.LogMessage(TracingLevel.Info, "Key Pressed");
        }

        public override async void KeyReleased(KeyPayload payload)
        {
            var rand = RandomGenerator.Next(100).ToString();
            
            // Just some example busy work to do when the button is released
            var tp = new TitleParameters()
            {
                FontFamily = SKTypeface.FromFamilyName("Arial"),
                FontStyle = SKFontStyle.Bold,
                FontSizeInPoints = 9f,
                TitleColor = SKColors.White
            };
            
            using (SKData data = Tools.GenerateKeyImage(tp, rand, SKColors.Black))
            {
                await Connection.SetImageAsync(data);
            }
            
            Logger.Instance.LogMessage(TracingLevel.Info, "Key Released");
        }

        public override void OnTick() { }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            Tools.AutoPopulateSettings(settings, payload.Settings);
            SaveSettings();
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

        #region Private Methods
        private Task SaveSettings()
        {
            return Connection.SetSettingsAsync(JObject.FromObject(settings));
        }
        #endregion
    }
}
