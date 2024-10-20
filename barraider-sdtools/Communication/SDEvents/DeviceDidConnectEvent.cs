using System.Text.Json.Serialization;
using BarRaider.SdTools.StreamDeckInfo;

namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// Payload for DeviceDidConnect Event
    /// </summary>
    public class DeviceDidConnectEvent : BaseEvent
    {
        /// <summary>
        /// UUID of device
        /// </summary>
        [JsonPropertyName("device")]
        public string Device { get; set; }

        /// <summary>
        /// Information on the device connected
        /// </summary>
        [JsonPropertyName("deviceInfo")]
        public StreamDeckDeviceInfo DeviceInfo { get; set; }
    }
}
