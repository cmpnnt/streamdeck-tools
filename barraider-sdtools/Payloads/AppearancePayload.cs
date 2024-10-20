using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using BarRaider.SdTools.Wrappers;

namespace BarRaider.SdTools.Payloads
{
    /// <summary>
    /// Payload for Apperance settings
    /// </summary>
    public class AppearancePayload
    {
        /// <summary>
        /// Additional settings
        /// </summary>
        [JsonPropertyName("settings")]
        public JsonObject Settings { get; private set; }

        /// <summary>
        /// Coordinates of key pressed
        /// </summary>
        [JsonPropertyName("coordinates")]
        public KeyCoordinates Coordinates { get; private set; }

        /// <summary>
        /// State of key
        /// </summary>
        [JsonPropertyName("state")]
        public uint State { get; private set; }

        /// <summary>
        /// Is action in MultiAction
        /// </summary>
        [JsonPropertyName("isInMultiAction")]
        public bool IsInMultiAction { get; private set; }

        /// <summary>
        /// Controller which issued the event
        /// </summary>
        [JsonPropertyName("controller")]
        public string Controller { get; private set; }
    }
}
