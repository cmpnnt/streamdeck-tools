﻿using BarRaider.SdTools.Payloads;
using Newtonsoft.Json;

namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// Payload for Dial up event
    /// </summary>
    public class DialUpEvent : BaseEvent
    {
        /// <summary>
        /// Action Name
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; private set; }

        /// <summary>
        /// Unique Action UUID
        /// </summary>
        [JsonProperty("context")]
        public string Context { get; private set; }

        /// <summary>
        /// Device UUID key was pressed on
        /// </summary>
        [JsonProperty("device")]
        public string Device { get; private set; }

        /// <summary>
        /// Information on dial status
        /// </summary>
        [JsonProperty("payload")]
        public DialPayload Payload { get; private set; }
    }
}
