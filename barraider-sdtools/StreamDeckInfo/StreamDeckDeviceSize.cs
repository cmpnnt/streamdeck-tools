using System.Text.Json.Serialization;

namespace BarRaider.SdTools.StreamDeckInfo
{
    /// <summary>
    /// Layout of the keys on the StreamDeck hardware device
    /// </summary>
    public record StreamDeckDeviceSize
    {
        /// <summary>
        /// Number of key rows on the StreamDeck hardware device
        /// </summary>
        [JsonPropertyName("rows")]
        public int Rows { get; set; }

        /// <summary>
        /// Number of key columns on the StreamDeck hardware device
        /// </summary>
        [JsonPropertyName("columns")]
        public int Cols { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        public StreamDeckDeviceSize(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
        }
    }
}
