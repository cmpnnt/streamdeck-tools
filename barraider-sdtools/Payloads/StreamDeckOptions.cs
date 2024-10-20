using System.Text.Json;
using BarRaider.SdTools.SourceGeneration;
using CommandLine;

namespace BarRaider.SdTools.Payloads
{
    /// <summary>
    /// Class holding all the information passed to the plugin when the program was launched
    /// </summary>
    public record StreamDeckOptions
    {
        /// <summary>
        /// Port to communicate with the StreamDeck app
        /// </summary>
        [Option("port", Required = true, HelpText = "The websocket port to connect to", SetName = "port")]
        public int Port { get; set; }

        /// <summary>
        /// UUID of the plugin
        /// </summary>
        [Option("pluginUUID", Required = true, HelpText = "The UUID of the plugin")]
        public string PluginUuid { get; set; }

        /// <summary>
        /// Name of the event we should pass to the StreamDeck app to register
        /// </summary>
        [Option("registerEvent", Required = true, HelpText = "The event triggered when the plugin is registered?")]
        public string RegisterEvent { get; set; }

        /// <summary>
        /// Raw information in JSON format which we will parse into the DeviceInfo property
        /// </summary>
        [Option("info", Required = true, HelpText = "Extra JSON launch data")]
        public string RawInfo { get; set; }

        /// <summary>
        /// Information regarding the StreamDeck app and StreamDeck hardware which was parsed from the RawInfo JSON field.
        /// </summary>
        public StreamDeckInfo.StreamDeckInfo DeviceInfo =>
            RawInfo == null ? null : JsonSerializer.Deserialize(RawInfo, SourceGenerationContext.Default.StreamDeckInfo);
    }
}
