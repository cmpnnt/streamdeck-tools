using Newtonsoft.Json;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class GetGlobalSettingsMessage(string pluginUuid) : IMessage
    {
        [JsonProperty("event")]
        public string Event => "getGlobalSettings";

        [JsonProperty("context")]
        public string Context { get; private set; } = pluginUuid;
    }
}
