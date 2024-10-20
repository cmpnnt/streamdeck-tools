using System.Text.Json.Serialization;
using BarRaider.SdTools.Payloads;


namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// Payload for TitleParametersDidChangeEvent event
    /// </summary>
    public class TitleParametersDidChangeEvent : BaseEvent
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
        /// Title settings
        /// </summary>
        [JsonPropertyName("payload")]
        public TitleParametersPayload Payload { get; private set; }
    }
}
