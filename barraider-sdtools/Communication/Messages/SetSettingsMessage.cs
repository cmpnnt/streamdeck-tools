using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SetSettingsMessage : IMessage
    {
        [JsonPropertyName("event")]
        public string Event { get { return "setSettings"; } }

        [JsonPropertyName("context")]
        public string Context { get; private set; }

        [JsonPropertyName("payload")]
        public JsonObject Payload { get; private set; }

        public SetSettingsMessage(JsonObject settings, string context)
        {
            Context = context;
            Payload = settings;
        }
    }
}
