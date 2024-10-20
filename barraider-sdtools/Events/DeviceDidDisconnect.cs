using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Events
{
    /// <summary>
    /// Payload for DeviceDidDisconnect event
    /// </summary>
    public class DeviceDidDisconnect
    {
        /// <summary>
        /// Device GUID
        /// </summary>
        [JsonPropertyName("device")]
        public string Device { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="device"></param>
        public DeviceDidDisconnect(string device)
        {
            Device = device;
        }
    }
}
