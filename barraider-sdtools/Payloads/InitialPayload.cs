using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using BarRaider.SdTools.Wrappers;

namespace BarRaider.SdTools.Payloads
{
    /// <summary>
    /// Payload received during the plugin's constructor
    /// </summary>
    public class InitialPayload
    {
        /// <summary>
        /// Plugin instance's settings (set through Property Inspector)
        /// </summary>
        [JsonPropertyName("settings")]
        public JsonObject Settings { get; private set; }

        /// <summary>
        /// Plugin's physical location on the Stream Deck device
        /// </summary>
        [JsonPropertyName("coordinates")]
        public KeyCoordinates Coordinates { get; private set; }

        /// <summary>
        /// Current plugin state
        /// </summary>
        [JsonPropertyName("state")]
        public uint State { get; private set; }

        /// <summary>
        /// Is it in a Multiaction
        /// </summary>
        [JsonPropertyName("isInMultiAction")]
        public bool IsInMultiAction { get; private set; }

        /// <summary>
        /// The controller of the current action. Values include "Keypad" and "Encoder".
        /// </summary>
        [JsonPropertyName("controller")]
        public string Controller { get; private set; }

        /// <summary>
        /// Information regarding the Stream Deck hardware device
        /// </summary>
        [JsonPropertyName("deviceInfo")]
        [JsonRequired]
        public StreamDeckInfo.StreamDeckInfo DeviceInfo { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="appearancePayload"></param>
        /// <param name="deviceInfo"></param>
        public InitialPayload(AppearancePayload appearancePayload, StreamDeckInfo.StreamDeckInfo deviceInfo)
        {
            Coordinates = appearancePayload.Coordinates;
            Settings = appearancePayload.Settings;
            State = appearancePayload.State;
            IsInMultiAction = appearancePayload.IsInMultiAction;
            Controller = appearancePayload.Controller;
            DeviceInfo = deviceInfo;
        }
    }
}
