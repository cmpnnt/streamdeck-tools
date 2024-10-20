using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class LogMessage : IMessage
    {
        [JsonPropertyName("event")]
        public string Event { get { return "logMessage"; } }

        [JsonPropertyName("payload")]
        public IPayload Payload { get; private set; }

        public LogMessage(string message)
        {
            Payload = new PayloadClass(message);
        }

        private class PayloadClass : IPayload
        {
            [JsonPropertyName("message")]
            public string Message { get; private set; }

            public PayloadClass(string message)
            {
                Message = message;
            }
        }
    }
}
