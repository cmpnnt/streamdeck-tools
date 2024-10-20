using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using BarRaider.SdTools.Wrappers;

namespace BarRaider.SdTools.Payloads
{
    /// <summary>
    /// Payload received when a key is pressed or released
    /// </summary>
    public record KeyPayload
    {
        /// <summary>
        /// Current event settings
        /// </summary>
        [JsonPropertyName("settings")]
        public JsonObject Settings { get; set; }

        /// <summary>
        /// Coordinates of key on the stream deck
        /// </summary>
        [JsonPropertyName("coordinates")]
        public KeyCoordinates Coordinates { get; set; }

        /// <summary>
        /// Current key state
        /// </summary>
        [JsonPropertyName("state")]
        public uint State { get; set; }

        /// <summary>
        /// Desired state
        /// </summary>
        [JsonPropertyName("userDesiredState")]
        public uint UserDesiredState { get; set; }

        /// <summary>
        /// Is part of a multiAction
        /// </summary>
        [JsonPropertyName("isInMultiAction")]
        public bool IsInMultiAction { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="settings"></param>
        /// <param name="state"></param>
        /// <param name="userDesiredState"></param>
        /// <param name="isInMultiAction"></param>
        public KeyPayload(KeyCoordinates coordinates, JsonObject settings, uint state, uint userDesiredState, bool isInMultiAction)
        {
            Coordinates = coordinates;
            Settings = settings;
            State = state;
            UserDesiredState = userDesiredState;
            IsInMultiAction = isInMultiAction;
        }

        /// <summary>
        /// For Serialization
        /// </summary>
        public KeyPayload() { }
    }
}
