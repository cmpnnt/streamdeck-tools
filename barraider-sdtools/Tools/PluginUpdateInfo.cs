using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Tools
{
    /// <summary>
    /// Status of update request
    /// </summary>
    public enum PluginUpdateStatus
    {
        /// <summary>
        /// Unknown reply
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Plugin is up to date
        /// </summary>
        UpToDate = 1,

        /// <summary>
        /// Minor version update available
        /// </summary>
        MinorUpgrade = 2,

        /// <summary>
        /// Major version update available
        /// </summary>
        MajorUpgrade = 3,

        /// <summary>
        /// Critical update needed
        /// </summary>
        CriticalUpgrade = 4
    }

    /// <summary>
    /// Response to an update request
    /// </summary>
    public class PluginUpdateInfo
    {
        /// <summary>
        /// Status
        /// </summary>
        [JsonPropertyName("status")]
        public PluginUpdateStatus Status { get; set; }

        /// <summary>
        /// Update URL
        /// </summary>
        [JsonPropertyName("updateURL")]
        public string UpdateUrl { get; set; }

        /// <summary>
        /// Update URL
        /// </summary>
        [JsonPropertyName("updateImage")]
        public string UpdateImage { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public PluginUpdateInfo()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="status"></param>
        /// <param name="updateUrl"></param>
        /// /// <param name="updateImage"></param>
        public PluginUpdateInfo(PluginUpdateStatus status, string updateUrl, string updateImage)
        {
            Status = status;
            UpdateUrl = updateUrl;
            UpdateImage = updateImage;
        }
    }
}
