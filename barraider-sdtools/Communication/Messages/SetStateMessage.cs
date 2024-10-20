using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SetStateMessage : IMessage
    {
        [JsonPropertyName("event")]
        public string Event { get { return "setState"; } }

        [JsonPropertyName("context")]
        public string Context { get; set; }

        [JsonPropertyName("payload")]
        public IPayload Payload { get; set; }

        public SetStateMessage(uint state, string context)
        {
            Context = context;
            Payload = new PayloadClass(state);
        }

        private class PayloadClass : IPayload
        {
            [JsonPropertyName("state")]
            public uint State { get; set; }

            public PayloadClass(uint state)
            {
                State = state;
            }
        }
    }
}
