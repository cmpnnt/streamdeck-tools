using System.Text.Json.Serialization;

namespace BarRaider.SdTools.StreamDeckInfo
{
    /// <summary>
    /// Class which holds information on the StreamDeck hardware device
    /// </summary>
    public record StreamDeckDeviceInfo
    {
        /// <summary>
        /// ID of the StreamDeck hardware device
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        /// <summary>
        /// Name of the StreamDeck hardware device
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// Details on number of keys of the StreamDeck hardware device
        /// </summary>
        [JsonPropertyName("size")]
        public StreamDeckDeviceSize Size { get; set; }

        /// <summary>
        /// Type of StreamDeck hardware device
        /// </summary>
        [JsonPropertyName("type")]
        public DeviceType Type { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public StreamDeckDeviceInfo(StreamDeckDeviceSize size, DeviceType type, string id, string name)
        {
            Size = size;
            Type = type;
            Id = id;
            Name = name;
        }
    }
}
