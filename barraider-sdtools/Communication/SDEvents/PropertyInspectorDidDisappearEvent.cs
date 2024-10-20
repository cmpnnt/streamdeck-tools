using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// Payload for PropertyInspectorDidDisappearEvent event
    /// </summary>
    public class PropertyInspectorDidDisappearEvent : BaseEvent
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
        /// Stream Deck device UUID
        /// </summary>
        [JsonPropertyName("device")]
        public string Device { get; private set; }
    }
}
