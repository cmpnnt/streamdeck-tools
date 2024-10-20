using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Events
{
    /// <summary>
    /// Payload for SendToPlugin event
    /// </summary>
    public class SendToPlugin
    {
        /// <summary>
        /// ActionId
        /// </summary>
        [JsonPropertyName("action")]
        public string Action { get; set; }

        /// <summary>
        /// ContextId
        /// </summary>
        [JsonPropertyName("context")]
        public string Context { get; set; }

        /// <summary>
        /// Payload
        /// </summary>
        [JsonPropertyName("payload")]
        public JsonObject Payload { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action"></param>
        /// <param name="context"></param>
        /// <param name="payload"></param>
        public SendToPlugin(string action, string context, JsonObject payload)
        {
            Action = action;
            Context = context;
            Payload = payload;
        }
    }
}
