﻿using Newtonsoft.Json;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SetImageMessage(string base64Image, string context, SdkTarget target, int? state)
        : IMessage
    {
        [JsonProperty("event")]
        public string Event => "setImage";

        [JsonProperty("context")]
        public string Context { get; private set; } = context;

        [JsonProperty("payload")]
        public IPayload Payload { get; private set; } = new PayloadClass(base64Image, target, state);

        private class PayloadClass(string image, SdkTarget target, int? state) : IPayload
        {
            [JsonProperty("image")]
            public string Image { get; private set; } = image;

            [JsonProperty("target")]
            public SdkTarget Target { get; private set; } = target;

            [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
            public int? State { get; private set; } = state;
        }
    }
}
