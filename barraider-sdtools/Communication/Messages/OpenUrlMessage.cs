using System;
using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class OpenUrlMessage : IMessage
    {
        [JsonPropertyName("event")]
        public string Event { get { return "openUrl"; } }

        [JsonPropertyName("payload")]
        public IPayload Payload { get; private set; }

        public OpenUrlMessage(Uri uri)
        {
            Payload = new PayloadClass(uri);
        }

        private class PayloadClass : IPayload
        {
            [JsonPropertyName("url")]
            public string Url { get; private set; }

            public PayloadClass(Uri uri)
            {
                Url = uri.ToString();
            }
        }
    }
}
