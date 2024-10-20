using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using BarRaider.SdTools.Wrappers;

namespace BarRaider.SdTools.Payloads
{
    /// <summary>
    /// Payload received when a key is pressed or released
    /// </summary>
    public class KeyPayload
    {
        /// <summary>
        /// Current event settings
        /// </summary>
        [JsonPropertyName("settings")]
        public JsonObject Settings { get; private set; }

        /// <summary>
        /// Coordinates of key on the stream deck
        /// </summary>
        [JsonPropertyName("coordinates")]
        public KeyCoordinates Coordinates { get; private set; }

        /// <summary>
        /// Current key state
        /// </summary>
        [JsonPropertyName("state")]
        public uint State { get; private set; }

        /// <summary>
        /// Desired state
        /// </summary>
        [JsonPropertyName("userDesiredState")]
        public uint UserDesiredState { get; private set; }

        /// <summary>
        /// Is part of a multiAction
        /// </summary>
        [JsonPropertyName("isInMultiAction")]
        public bool IsInMultiAction { get; private set; }

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
        /// For Seralization
        /// </summary>
        public KeyPayload() { }
    }
}
