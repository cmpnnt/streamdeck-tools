using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SetGlobalSettingsMessage(JObject settings, string pluginUuid) : IMessage
    {
        [JsonProperty("event")]
        public string Event => "setGlobalSettings";

        [JsonProperty("context")]
        public string Context { get; private set; } = pluginUuid;

        [JsonProperty("payload")]
        public JObject Payload { get; private set; } = settings;
    }
}
