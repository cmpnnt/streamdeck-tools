using System.Text.Json.Serialization;
using BarRaider.SdTools.StreamDeckInfo;

namespace BarRaider.SdTools.Events
{
    /// <summary>
    /// Payload for DeviceDidConnect event
    /// </summary>
    public class DeviceDidConnect
    {
        /// <summary>
        /// Device GUID
        /// </summary>
        [JsonPropertyName("device")]
        public string Device { get; set; }

        /// <summary>
        /// Device Info
        /// </summary>
        [JsonPropertyName("deviceInfo")]
        public StreamDeckDeviceInfo DeviceInfo { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deviceInfo"></param>
        public DeviceDidConnect(StreamDeckDeviceInfo deviceInfo)
        {
            Device = deviceInfo?.Id;
            DeviceInfo = deviceInfo;
        }
    }
}
