using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SetStateMessage : IMessage
    {
        [JsonPropertyName("event")]
        public string Event { get { return "setState"; } }

        [JsonPropertyName("context")]
        public string Context { get; private set; }

        [JsonPropertyName("payload")]
        public IPayload Payload { get; private set; }

        public SetStateMessage(uint state, string context)
        {
            Context = context;
            Payload = new PayloadClass(state);
        }

        private class PayloadClass : IPayload
        {
            [JsonPropertyName("state")]
            public uint State { get; private set; }

            public PayloadClass(uint state)
            {
                State = state;
            }
        }
    }
}
