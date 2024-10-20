using System.Text.Json.Serialization;

namespace BarRaider.SdTools.StreamDeckInfo
{
    /// <summary>
    /// Holds general information on the StreamDeck App we're communicating with
    /// </summary>
    public record StreamDeckPluginInfo
    {
        /// <summary>
        /// Unique identifier of the plugin
        /// </summary>
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }
        
        /// <summary>
        /// Current version of the plugin
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }
    }
}
