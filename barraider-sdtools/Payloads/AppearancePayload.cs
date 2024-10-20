using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using BarRaider.SdTools.Wrappers;

namespace BarRaider.SdTools.Payloads
{
    /// <summary>
    /// Payload for Apperance settings
    /// </summary>
    public record AppearancePayload
    {
        /// <summary>
        /// Additional settings
        /// </summary>
        [JsonPropertyName("settings")]
        public JsonObject Settings { get; set; }

        /// <summary>
        /// Coordinates of key pressed
        /// </summary>
        [JsonPropertyName("coordinates")]
        public KeyCoordinates Coordinates { get; set; }

        /// <summary>
        /// State of key
        /// </summary>
        [JsonPropertyName("state")]
        public uint State { get; set; }

        /// <summary>
        /// Is action in MultiAction
        /// </summary>
        [JsonPropertyName("isInMultiAction")]
        public bool IsInMultiAction { get; set; }

        /// <summary>
        /// Controller which issued the event
        /// </summary>
        [JsonPropertyName("controller")]
        public string Controller { get; set; }
    }
}
