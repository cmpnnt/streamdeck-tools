using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SetFeedbackMessageEx : IMessage
    {
        [JsonPropertyName("event")]
        public string Event { get { return "setFeedback"; } }

        [JsonPropertyName("context")]
        public string Context { get; set; }

        [JsonPropertyName("payload")]
        public JsonObject Payload { get; set; }

        public SetFeedbackMessageEx(JsonObject payload, string pluginUuid)
        {
            Context = pluginUuid;
            Payload = payload;
        }
    }
}
