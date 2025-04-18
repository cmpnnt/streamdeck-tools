﻿using BarRaider.SdTools.Wrappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BarRaider.SdTools.Payloads
{
    /// <summary>
    /// Payload that holds all the settings in the ReceivedSettings event
    /// </summary>
    public class ReceivedSettingsPayload
    {
        /// <summary>
        /// Action's settings
        /// </summary>
        [JsonProperty("settings")]
        public JObject Settings { get; private set; }

        /// <summary>
        /// Coordinates of the key pressed
        /// </summary>
        [JsonProperty("coordinates")]
        public KeyCoordinates Coordinates { get; private set; }

        /// <summary>
        /// Is event part of a multiaction
        /// </summary>
        [JsonProperty("isInMultiAction")]
        public bool IsInMultiAction { get; private set; }
    }
}
