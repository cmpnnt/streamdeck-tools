using System.Text.Json.Serialization;

namespace BarRaider.SdTools.StreamDeckInfo
{
    /// <summary>
    /// Holds general information on the StreamDeck App we're communicating with
    /// </summary>
    public class StreamDeckApplicationInfo
    {
        /// <summary>
        /// Current language of the StreamDeck app
        /// </summary>
        [JsonPropertyName("language")]
        public string Language { get; private set; }

        /// <summary>
        /// OS Platform
        /// </summary>
        [JsonPropertyName("platform")]
        public string Platform { get; private set; }

        /// <summary>
        /// Current version of the StreamDeck app
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; private set; }

        /// <summary>
        /// Shows class information as string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Language: {Language} Platform: {Platform} Version: {Version}";
        }
    }
}
