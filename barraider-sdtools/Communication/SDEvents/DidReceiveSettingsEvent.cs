using System.Text.Json.Serialization;
using BarRaider.SdTools.Payloads;


namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// Payload for DidReceiveSettings Event
    /// </summary>
    public class DidReceiveSettingsEvent : BaseEvent
    {
        /// <summary>
        /// Action Name
        /// </summary>
        [JsonPropertyName("action")]
        public string Action { get; private set; }

        /// <summary>
        /// Context (unique action UUID)
        /// </summary>
        [JsonPropertyName("context")]
        public string Context { get; private set; }

        /// <summary>
        /// Device UUID action is on
        /// </summary>
        [JsonPropertyName("device")]
        public string Device { get; private set; }

        /// <summary>
        /// Settings for action
        /// </summary>
        [JsonPropertyName("payload")]
        public ReceivedSettingsPayload Payload { get; private set; }
    }
}
