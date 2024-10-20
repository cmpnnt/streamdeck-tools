using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SwitchToProfileMessage : IMessage
    {
        [JsonPropertyName("event")]
        public string Event { get { return "switchToProfile"; } }

        [JsonPropertyName("context")]
        public string Context { get; set; }

        [JsonPropertyName("device")]
        public string Device { get; set; }

        [JsonPropertyName("payload")]
        public IPayload Payload { get; set; }

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

        private class PayloadClass : IPayload
        {
            [JsonPropertyName("profile")]
            public string Profile { get; set; }

            public PayloadClass(string profile)
            {
                Profile = profile;
            }
        }
    }
}
