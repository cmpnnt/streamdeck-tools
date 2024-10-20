using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using BarRaider.SdTools.Wrappers;

namespace BarRaider.SdTools.Payloads
{
    /// <summary>
    /// Payload that holds all the settings in the ReceivedSettings event
    /// </summary>
    public class ReceivedSettingsPayload
    {
        /// <summary>
        /// Action's settings
        /// </summary>
        [JsonPropertyName("settings")]
        public JsonObject Settings { get; private set; }

        /// <summary>
        /// Coordinates of the key pressed
        /// </summary>
        [JsonPropertyName("coordinates")]
        public KeyCoordinates Coordinates { get; private set; }

        /// <summary>
        /// Is event part of a multiaction
        /// </summary>
        [JsonPropertyName("isInMultiAction")]
        public bool IsInMultiAction { get; private set; }
    }
}
