using System.Text.Json.Serialization;

namespace BarRaider.SdTools.StreamDeckInfo
{
    /// <summary>
    /// Holds general information on the StreamDeck App we're communicating with
    /// </summary>
    public record StreamDeckApplicationInfo
    {
        /// <summary>
        /// Current font of the plugin
        /// </summary>
        [JsonPropertyName("font")]
        public string Font { get; set; }
        
        /// <summary>
        /// Current language of the StreamDeck app
        /// </summary>
        [JsonPropertyName("language")]
        public string Language { get; set; }

        /// <summary>
        /// OS Platform
        /// </summary>
        [JsonPropertyName("platform")]
        public string Platform { get; set; }

        /// <summary>
        /// Current version of the StreamDeck app
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }
        
        /// <summary>
        /// Current platform version of the StreamDeck platform
        /// </summary>
        [JsonPropertyName("platformVersion")]
        public string PlatformVersion { get; set; }
    }
}
