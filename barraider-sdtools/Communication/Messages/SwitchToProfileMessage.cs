using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SwitchToProfileMessage : IMessage
    {
        [JsonPropertyName("event")]
        public string Event { get { return "switchToProfile"; } }

        [JsonPropertyName("context")]
        public string Context { get; private set; }

        [JsonPropertyName("device")]
        public string Device { get; private set; }

        [JsonPropertyName("payload")]
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

        private class PayloadClass : IPayload
        {
            [JsonPropertyName("profile")]
            public string Profile { get; private set; }

            public PayloadClass(string profile)
            {
                Profile = profile;
            }
        }
    }
}
