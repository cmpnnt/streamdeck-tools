using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SetTitleMessage : IMessage
    {
        [JsonPropertyName("event")]
        public string Event { get { return "setTitle"; } }

        [JsonPropertyName("context")]
        public string Context { get; set; }

        [JsonPropertyName("payload")]
        public IPayload Payload { get; set; }

        public SetTitleMessage(string title, string context, SdkTarget target, int? state)
        {
            Context = context;
            Payload = new PayloadClass(title, target, state);
        }

        private class PayloadClass : IPayload
        {
            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("target")]
            public SdkTarget Target { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public int? State { get; set; }

            public PayloadClass(string title, SdkTarget target, int? state)
            {
                Title = title;
                Target = target;
                State = state;
            }
        }
    }
}
