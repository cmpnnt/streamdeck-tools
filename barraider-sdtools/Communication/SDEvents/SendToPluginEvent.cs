using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// Payload for SendToPluginEvent event
    /// </summary>
    public class SendToPluginEvent : BaseEvent
    {
        /// <summary>
        /// Action Name
        /// </summary>
        [JsonPropertyName("action")]
        public string Action { get; private set; }

        /// <summary>
        /// Unique Action UUID
        /// </summary>
        [JsonPropertyName("context")]
        public string Context { get; private set; }

        /// <summary>
        /// Payload
        /// </summary>
        [JsonPropertyName("payload")]
        public JsonObject Payload { get; private set; }
    }
}
