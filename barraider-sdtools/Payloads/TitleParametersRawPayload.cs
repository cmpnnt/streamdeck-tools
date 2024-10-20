using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Payloads
{
    /// <summary>
    /// Raw payload for TitleParametersRawPayload event (without objects)
    /// </summary>
    public class TitleParametersRawPayload
    {
        /// <summary>
        /// Name of font family
        /// </summary>
        [JsonPropertyName("fontFamily")]
        public string FontFamily { get; private set; }

        /// <summary>
        /// Size of font
        /// </summary>
        [JsonPropertyName("fontSize")]
        public uint FontSize { get; private set; }

        /// <summary>
        /// Style of font (bold, italic)
        /// </summary>
        [JsonPropertyName("fontStyle")]
        public string FontStyle { get; private set; }

        /// <summary>
        /// Is there an underling
        /// </summary>
        [JsonPropertyName("fontUnderline")]
        public bool FontUnderline { get; private set; }

        /// <summary>
        /// Should title be shown
        /// </summary>
        [JsonPropertyName("showTitle")]
        public bool ShowTitle { get; private set; }

        /// <summary>
        /// Alignment of title (top, middle, bottom)
        /// </summary>
        [JsonPropertyName("titleAlignment")]
        public string TitleAlignment { get; private set; }

        /// <summary>
        /// Color of title
        /// </summary>
        [JsonPropertyName("titleColor")]
        public string TitleColor { get; private set; }
    }
}
