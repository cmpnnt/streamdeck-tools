using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SetTitleMessage : IMessage
    {
        [JsonPropertyName("event")]
        public string Event { get { return "setTitle"; } }

        [JsonPropertyName("context")]
        public string Context { get; private set; }

        [JsonPropertyName("payload")]
        public IPayload Payload { get; private set; }

        public SetTitleMessage(string title, string context, SdkTarget target, int? state)
        {
            Context = context;
            Payload = new PayloadClass(title, target, state);
        }

        private class PayloadClass : IPayload
        {
            [JsonPropertyName("title")]
            public string Title { get; private set; }

            [JsonPropertyName("target")]
            public SdkTarget Target { get; private set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public int? State { get; private set; }

            public PayloadClass(string title, SdkTarget target, int? state)
            {
                Title = title;
                Target = target;
                State = state;
            }
        }
    }
}
