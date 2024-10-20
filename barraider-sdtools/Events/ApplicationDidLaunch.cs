using System.Text.Json.Serialization;
using BarRaider.SdTools.Payloads;


namespace BarRaider.SdTools.Events
{
    /// <summary>
    /// Payload for ApplicationDidLaunch event
    /// </summary>
    public class ApplicationDidLaunch
    {
        /// <summary>
        /// Payload
        /// </summary>
        [JsonPropertyName("payload")]
        public ApplicationPayload Payload { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payload"></param>
        public ApplicationDidLaunch(ApplicationPayload payload)
        {
            Payload = payload;
        }
    }
}
