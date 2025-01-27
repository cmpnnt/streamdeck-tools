using System.Text;
using Newtonsoft.Json;

namespace BarRaider.SdTools.StreamDeckInfo
{
    /// <summary>
    /// Class which holds information on the StreamDeck app and StreamDeck hardware device that the plugin is communicating with
    /// </summary>
    public class RegistrationInfo
    {
        /// <summary>
        /// Information on the StreamDeck App which we're communicating with
        /// </summary>
        [JsonProperty(PropertyName = "application")]
        public StreamDeckApplicationInfo Application { get; private set; }

        /// <summary>
        /// Information on the StreamDeck hardware device that the plugin is running on
        /// </summary>
        [JsonProperty(PropertyName = "devices")]
        public StreamDeckDeviceInfo[] Devices { get; private set; }

        /// <summary>
        /// Information on the Plugin we're currently running
        /// </summary>
        [JsonProperty(PropertyName = "plugin")]
        public StreamDeckPluginInfo Plugin { get; private set; }

        /// <summary>
        /// Device pixel ratio
        /// </summary>
        [JsonProperty(PropertyName = "devicePixelRatio")]
        public int DevicePixelRatio { get; private set; }

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
                foreach (StreamDeckDeviceInfo sdi in Devices)
                {
                    if (sdi != null)
                    {
                        sb.Append($"[{sdi}]\n");
                    }
                }
            }

            if (Application != null) sb.Append($"ApplicationInfo: {Application}\n");
            if (Plugin != null) sb.Append($"PluginInfo: {Plugin}\n");
            
            return sb.ToString();
        }
    }
}
