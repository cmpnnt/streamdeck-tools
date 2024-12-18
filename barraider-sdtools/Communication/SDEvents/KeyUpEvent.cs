﻿using System.Text.Json.Serialization;
using BarRaider.SdTools.Payloads;


namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// Payload for KeyUp event
    /// </summary>
    public class KeyUpEvent : BaseEvent
    {
        /// <summary>
        /// Action name
        /// </summary>
        [JsonPropertyName("action")]
        public string Action { get; set; }

        /// <summary>
        /// Unique action UUID
        /// </summary>
        [JsonPropertyName("context")]
        public string Context { get; set; }

        /// <summary>
        /// Stream Deck device UUID
        /// </summary>
        [JsonPropertyName("device")]
        public string Device { get; set; }

        /// <summary>
        /// Key settings
        /// </summary>
        [JsonPropertyName("payload")]
        public KeyPayload Payload { get; set; }
    }
}
