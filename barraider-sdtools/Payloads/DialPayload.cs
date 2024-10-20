﻿using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using BarRaider.SdTools.Wrappers;

namespace BarRaider.SdTools.Payloads
{
    /// <summary>
    /// Payload received when a dial is down or up
    /// </summary>
    public class DialPayload
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
        /// Constructor
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="settings"></param>
        /// <param name="controller"></param>
        public DialPayload(KeyCoordinates coordinates, JsonObject settings, string controller)
        {
            Coordinates = coordinates;
            Settings = settings;
            Controller = controller;
        }

        /// <summary>
        /// Default constructor for serialization
        /// </summary>
        public DialPayload() { }
    }
}
