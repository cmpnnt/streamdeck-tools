using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class ShowOkMessage : IMessage
    {
        [JsonPropertyName("event")]
        public string Event { get { return "showOk"; } }

        [JsonPropertyName("context")]
        public string Context { get; private set; }

        public ShowOkMessage(string context)
        {
            Context = context;
        }
    }
}
