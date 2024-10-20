using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Payloads
{
    /// <summary>
    /// ApplicationPayload
    /// </summary>
    public record ApplicationPayload
    {
        /// <summary>
        /// Application Name
        /// </summary>
        [JsonPropertyName("application")]
        public string Application { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="application"></param>
        public ApplicationPayload(string application)
        {
            Application = application;
        }
    }
}
