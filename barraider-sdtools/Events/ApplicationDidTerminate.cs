﻿using System.Text.Json.Serialization;
using BarRaider.SdTools.Payloads;


namespace BarRaider.SdTools.Events
{
    /// <summary>
    /// Payload for ApplicationDidTerminate event
    /// </summary>
    public class ApplicationDidTerminate
    {
        /// <summary>
        /// Payload
        /// </summary>
        [JsonPropertyName("payload")]
        public ApplicationPayload Payload { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payload"></param>
        public ApplicationDidTerminate(ApplicationPayload payload)
        {
            Payload = payload;
        }
    }
}
