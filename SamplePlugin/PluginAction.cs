using BarRaider.SdTools.Wrappers;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BarRaider.SdTools.Attributes;
using BarRaider.SdTools.Backend;
using BarRaider.SdTools.Payloads;
using BarRaider.SdTools.Tools;

namespace SamplePlugin
{
    [PluginActionId("com.test.sdtools.sampleplugin")]
    public class PluginAction : KeypadBase
    {
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
            [JsonPropertyName("outputFileName")]
            public string OutputFileName { get; set; }

            [JsonPropertyName("inputString")]
            public string InputString { get; set; }
        }

        #region Private Members

        private readonly PluginSettings settings;

        #endregion
        public PluginAction(ISdConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                settings = PluginSettings.CreateDefaultSettings();
            }
            else
            {
                settings = payload.Settings.Deserialize<PluginSettings>();
            }

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
        }

        private void Connection_OnSendToPlugin(object sender, SdEventReceivedEventArgs<BarRaider.SdTools.Events.SendToPlugin> e)
        {
        }

        private void Connection_OnPropertyInspectorDidDisappear(object sender, SdEventReceivedEventArgs<BarRaider.SdTools.Events.PropertyInspectorDidDisappear> e)
        {
        }

        private void Connection_OnPropertyInspectorDidAppear(object sender, SdEventReceivedEventArgs<BarRaider.SdTools.Events.PropertyInspectorDidAppear> e)
        {
        }

        private void Connection_OnDeviceDidDisconnect(object sender, SdEventReceivedEventArgs<BarRaider.SdTools.Events.DeviceDidDisconnect> e)
        {
        }

        private void Connection_OnDeviceDidConnect(object sender, SdEventReceivedEventArgs<BarRaider.SdTools.Events.DeviceDidConnect> e)
        {
        }

        private void Connection_OnApplicationDidTerminate(object sender, SdEventReceivedEventArgs<BarRaider.SdTools.Events.ApplicationDidTerminate> e)
        {
        }

        private void Connection_OnApplicationDidLaunch(object sender, SdEventReceivedEventArgs<BarRaider.SdTools.Events.ApplicationDidLaunch> e)
        {
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

        public override async void KeyPressed(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.Info, "Key Pressed");
            var tp = new TitleParameters(
                new FontFamily("Arial"),
                FontStyle.Bold,
                20,
                Color.White,
                true,
                TitleVerticalAlignment.Middle);
            using Image image = Tools.GenerateGenericKeyImage(out Graphics graphics);
            graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, image.Width, image.Height);
            graphics.AddTextPath(tp, image.Height, image.Width, "Test");
            graphics.Dispose();

            await Connection.SetImageAsync(image);
        }

        public override async void KeyReleased(KeyPayload payload) 
        {
            var tp = new TitleParameters(
                new FontFamily("Arial"),
                FontStyle.Bold,
                20,
                Color.White,
                true,
                TitleVerticalAlignment.Middle);
            using Image image = Tools.GenerateGenericKeyImage(out Graphics graphics);
            graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, image.Width, image.Height);
            graphics.AddTextPath(tp, image.Height, image.Width, "Test", Color.Black, 7);
            graphics.Dispose();

            await Connection.SetImageAsync(image);
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
            return Connection.SetSettingsAsync((JsonObject)JsonSerializer.SerializeToNode(settings));
        }

        #endregion
    }
}