using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using BarRaider.SdTools.Wrappers;

namespace BarRaider.SdTools.Payloads
{
    /// <summary>
    /// Payload received when the touchpad (above the dials) is pressed
    /// </summary>
    public record TouchpadPressPayload
    {
        /// <summary>
        /// Controller which issued the event
        /// </summary>
        [JsonPropertyName("controller")]
        public string Controller { get; set; }

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
        /// Boolean whether it was a long press or not
        /// </summary>
        [JsonPropertyName("hold")]
        public bool IsLongPress { get; set; }

        /// <summary>
        /// Position on touchpad which was pressed
        /// </summary>
        [JsonPropertyName("tapPos")]
        public int[] TapPosition { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="settings"></param>
        /// <param name="controller"></param>
        /// <param name="isLongPress"></param>
        /// <param name="tapPosition"></param>
        public TouchpadPressPayload(KeyCoordinates coordinates, JsonObject settings, string controller, bool isLongPress, int[] tapPosition)
        {
            Coordinates = coordinates;
            Settings = settings;
            Controller = controller;
            IsLongPress = isLongPress;
            TapPosition = tapPosition;
        }

        /// <summary>
        /// Default constructor for serialization
        /// </summary>
        public TouchpadPressPayload() { }
    }
}
