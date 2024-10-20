using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class ShowAlertMessage : IMessage
    {
        [JsonPropertyName("event")]
        public string Event { get { return "showAlert"; } }

        [JsonPropertyName("context")]
        public string Context { get; private set; }

        public ShowAlertMessage(string context)
        {
            Context = context;
        }
    }
}
