using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Wrappers
{
    /// <summary>
    /// Coordinates of the current key
    /// </summary>
    public record KeyCoordinates
    {
        /// <summary>
        /// Column of the current key
        /// </summary>
        [JsonPropertyName("column")]
        public int Column { get; set; }

        /// <summary>
        /// Row of the current key
        /// </summary>
        [JsonPropertyName("row")]
        public int Row { get; set; }
    }
}

