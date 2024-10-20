using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Payloads
{
    /// <summary>
    /// Payload that holds all the settings in the ReceivedGlobalSettings event
    /// </summary>
    public record ReceivedGlobalSettingsPayload
    {
        /// <summary>
        /// Global settings object
        /// </summary>
        [JsonPropertyName("settings")]
        public JsonObject Settings { get; set; }
    }
}
