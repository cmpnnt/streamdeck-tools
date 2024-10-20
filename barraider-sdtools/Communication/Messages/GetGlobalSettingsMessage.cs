using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class GetGlobalSettingsMessage : IMessage
    {
        [JsonPropertyName("event")]
        public string Event { get { return "getGlobalSettings"; } }

        [JsonPropertyName("context")]
        public string Context { get; private set; }

        public GetGlobalSettingsMessage(string pluginUuid)
        {
            Context = pluginUuid;
        }
    }
}
