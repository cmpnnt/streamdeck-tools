using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using BarRaider.SdTools.Wrappers;

namespace BarRaider.SdTools.Payloads
{
    /// <summary>
    /// Payload received when a dial is rotated
    /// </summary>
    public class DialRotatePayload
    {
        /// <summary>
        /// Controller which issued the event
        /// </summary>
        [JsonPropertyName("controller")]
        public string Controller { get; private set; }

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
        /// Number of ticks rotated. Positive is to the right, negative to the left
        /// </summary>
        [JsonPropertyName("ticks")]
        public int Ticks { get; private set; }

        /// <summary>
        /// Boolean whether the dial is currently pressed or not
        /// </summary>
        [JsonPropertyName("pressed")]
        public bool IsDialPressed { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="settings"></param>
        /// <param name="controller"></param>
        /// <param name="ticks"></param>
        /// <param name="isDialPressed"></param>
        public DialRotatePayload(KeyCoordinates coordinates, JsonObject settings, string controller, int ticks, bool isDialPressed)
        {
            Coordinates = coordinates;
            Settings = settings;
            Controller = controller;
            Ticks = ticks;
            IsDialPressed = isDialPressed;
        }

        /// <summary>
        /// Default constructor for serialization
        /// </summary>
        public DialRotatePayload() { }
    }
}
