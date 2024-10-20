using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// Payload for DeviceDidDisconnect Event
    /// </summary>
    public class DeviceDidDisconnectEvent : BaseEvent
    {
        /// <summary>
        /// UUID of device that was disconnected
        /// </summary>
        [JsonPropertyName("device")]
        public string Device { get; private set; }
    }
}
