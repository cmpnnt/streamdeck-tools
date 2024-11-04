using Newtonsoft.Json;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SwitchToProfileMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event => "switchToProfile";

        [JsonProperty("context")]
        public string Context { get; private set; }

        [JsonProperty("device")]
        public string Device { get; private set; }

        [JsonProperty("payload")]
        public IPayload Payload { get; private set; }

        public SwitchToProfileMessage(string device, string profileName, string pluginUuid)
        {
            Context = pluginUuid;
            Device = device;
            if (!string.IsNullOrEmpty(profileName))
            {
                Payload = new PayloadClass(profileName);
            }
            else
            {
                Payload = new EmptyPayload();
            }
        }

        private class PayloadClass(string profile) : IPayload
        {
            [JsonProperty("profile")]
            public string Profile { get; private set; } = profile;
        }
    }
}
