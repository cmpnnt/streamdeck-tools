using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class RegisterEventMessage : IMessage
    {
        [JsonPropertyName("event")]
        public string Event { get; private set; }

        [JsonPropertyName("uuid")]
        public string Uuid { get; private set; }

        public RegisterEventMessage(string eventName, string uuid)
        {
            Event = eventName;
            Uuid = uuid;
        }
    }
}
