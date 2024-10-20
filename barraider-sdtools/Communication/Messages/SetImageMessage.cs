using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SetImageMessage : IMessage
    {
        [JsonPropertyName("event")]
        public string Event { get { return "setImage"; } }

        [JsonPropertyName("context")]
        public string Context { get; set; }

        [JsonPropertyName("payload")]
        public IPayload Payload { get; set; }

        public SetImageMessage(string base64Image, string context, SdkTarget target, int? state)
        {
            Context = context;
            Payload = new PayloadClass(base64Image, target, state);
        }

        private class PayloadClass : IPayload
        {
            [JsonPropertyName("image")]
            public string Image { get; set; }

            [JsonPropertyName("target")]
            public SdkTarget Target { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public int? State { get; set; }

            public PayloadClass(string image, SdkTarget target, int? state)
            {
                Image = image;
                Target = target;
                State = state;
            }
        }
    }
}
