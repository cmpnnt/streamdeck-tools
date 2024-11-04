using Newtonsoft.Json;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class ShowAlertMessage(string context) : IMessage
    {
        [JsonProperty("event")]
        public string Event => "showAlert";

        [JsonProperty("context")]
        public string Context { get; private set; } = context;
    }
}
