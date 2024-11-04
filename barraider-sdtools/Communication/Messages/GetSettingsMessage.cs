using Newtonsoft.Json;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class GetSettingsMessage(string context) : IMessage
    {
        [JsonProperty("event")]
        public string Event => "getSettings";

        [JsonProperty("context")]
        public string Context { get; private set; } = context;
    }
}
