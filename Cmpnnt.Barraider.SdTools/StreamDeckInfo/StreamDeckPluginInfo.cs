﻿using Newtonsoft.Json;

namespace BarRaider.SdTools.StreamDeckInfo
{
    /// <summary>
    /// Holds general information on the StreamDeck App we're communicating with
    /// </summary>
    public class StreamDeckPluginInfo
    {
        /// <summary>
        /// Current version of the plugin
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public string Version { get; private set; }

        /// <summary>
        /// Shows class information as string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Version: {Version}";
        }
    }
}
