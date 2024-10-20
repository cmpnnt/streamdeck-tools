using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SetFeedbackLayoutMessage : IMessage
    {
        [JsonPropertyName("event")]
        public string Event { get { return "setFeedbackLayout"; } }

        [JsonPropertyName("context")]
        public string Context { get; set; }

        [JsonPropertyName("payload")]
        public IPayload Payload { get; set; }

        public SetFeedbackLayoutMessage(string layout, string context)
        {
            Context = context;
            Payload = new PayloadClass(layout);
        }

        private class PayloadClass : IPayload
        {
            [JsonPropertyName("layout")]
            public string Layout { get; set; }
            public PayloadClass(string layout)
            {
                Layout = layout;
            }
        }
    }
}
