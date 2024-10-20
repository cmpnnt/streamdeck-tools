using System.Text.Json.Serialization;
using BarRaider.SdTools.Payloads;

namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// Payload for Dial up event
    /// </summary>
    public class DialUpEvent : BaseEvent
    {
        /// <summary>
        /// Action Name
        /// </summary>
        [JsonPropertyName("action")]
        public string Action { get; set; }

        /// <summary>
        /// Unique Action UUID
        /// </summary>
        [JsonPropertyName("context")]
        public string Context { get; set; }

        /// <summary>
        /// Device UUID key was pressed on
        /// </summary>
        [JsonPropertyName("device")]
        public string Device { get; set; }

        /// <summary>
        /// Information on dial status
        /// </summary>
        [JsonPropertyName("payload")]
        public DialPayload Payload { get; set; }
    }
}
