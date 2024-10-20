using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class GetSettingsMessage : IMessage
    {
        [JsonPropertyName("event")]
        public string Event { get { return "getSettings"; } }

        [JsonPropertyName("context")]
        public string Context { get; private set; }

        public GetSettingsMessage(string context)
        {
            Context = context;
        }
    }
}
