using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using BarRaider.SdTools.Wrappers;

namespace BarRaider.SdTools.Payloads
{
    /// <summary>
    /// Payload received when the touchpad (above the dials) is pressed
    /// </summary>
    public class TouchpadPressPayload
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
        /// Boolean whether it was a long press or not
        /// </summary>
        [JsonPropertyName("hold")]
        public bool IsLongPress { get; private set; }

        /// <summary>
        /// Position on touchpad which was pressed
        /// </summary>
        [JsonPropertyName("tapPos")]
        public int[] TapPosition { get; private set; }


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
