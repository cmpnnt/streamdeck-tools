using System.Text;
using System.Text.Json.Serialization;

namespace BarRaider.SdTools.StreamDeckInfo
{
    /// <summary>
    /// Class which holds information on the StreamDeck app and StreamDeck hardware device that the plugin is communicating with
    /// </summary>
    public record StreamDeckInfo
    {
        /// <summary>
        /// Information on the StreamDeck App which we're communicating with
        /// </summary>
        [JsonPropertyName("application")]
        public StreamDeckApplicationInfo Application { get; set; }
        
        /// <summary>
        /// Collection of preferred colors used by the Stream Deck
        /// </summary>
        [JsonPropertyName("colors")]
        public StreamDeckColors Colors { get; set; }

        /// <summary>
        /// Device pixel ratio
        /// </summary>
        [JsonPropertyName("devicePixelRatio")]
        public int DevicePixelRatio { get; set; }
        
        /// <summary>
        /// Information on the StreamDeck hardware device that the plugin is running on
        /// </summary>
        [JsonPropertyName("devices")]
        public StreamDeckDeviceInfo[] Devices { get; set; }

        /// <summary>
        /// Information on the Plugin we're currently running
        /// </summary>
        [JsonPropertyName("plugin")]
        public StreamDeckPluginInfo Plugin { get; set; }
        
        /// <summary>
        /// Shows class information as string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Devices != null)
            {
                sb.Append("Devices:\n");
                foreach (StreamDeckDeviceInfo info in Devices)
                {
                    if (info != null)
                    {
                        sb.Append($"[{info}]\n");
                    }
                }
            }

            if (Application != null)
            {
                sb.Append($"ApplicationInfo: {Application}\n");
            }

            if (Plugin != null)
            {
                sb.Append($"PluginInfo: {Plugin}\n");
            }
            
            if (Colors != null)
            {
                sb.Append($"Colors: {Colors}\n");
            }
            
            return sb.ToString();
        }
    }
}
