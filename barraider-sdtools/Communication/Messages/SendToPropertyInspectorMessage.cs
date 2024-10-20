using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SendToPropertyInspectorMessage : IMessage
    {
        [JsonPropertyName("event")]
        public string Event { get { return "sendToPropertyInspector"; } }

        [JsonPropertyName("context")]
        public string Context { get; set; }

        [JsonPropertyName("payload")]
        public JsonObject Payload { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        public SendToPropertyInspectorMessage(string action, JsonObject data, string context)
        {
            Context = context;
            Payload = data;
            Action = action;
        }
    }
}
