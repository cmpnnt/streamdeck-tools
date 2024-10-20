using System.Text.Json.Serialization;
using BarRaider.SdTools.Payloads;


namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// Payload for WillDisappearEvent event
    /// </summary>
    public class WillDisappearEvent : BaseEvent
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

        /// <summary>
        /// settings
        /// </summary>
        [JsonPropertyName("payload")]
        public AppearancePayload Payload { get; private set; }
    }
}
