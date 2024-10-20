using System.Text.Json.Serialization;
using BarRaider.SdTools.Payloads;


namespace BarRaider.SdTools.Events
{
    /// <summary>
    /// Payload for TitleParametersDidChange event
    /// </summary>
    public class TitleParametersDidChange
    {
        /// <summary>
        /// Action Id
        /// </summary>
        [JsonPropertyName("action")]
        public string Action { get; set; }

        /// <summary>
        /// Context Id
        /// </summary>
        [JsonPropertyName("context")]
        public string Context { get; set; }

        /// <summary>
        /// Device Guid
        /// </summary>
        [JsonPropertyName("device")]
        public string Device { get; set; }

        /// <summary>
        /// Payload
        /// </summary>
        [JsonPropertyName("payload")]
        public TitleParametersPayload Payload { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action"></param>
        /// <param name="context"></param>
        /// <param name="device"></param>
        /// <param name="payload"></param>
        public TitleParametersDidChange(string action, string context, string device, TitleParametersPayload payload)
        {
            Action = action;
            Context = context;
            Device = device;
            Payload = payload;
        }
    }
}
