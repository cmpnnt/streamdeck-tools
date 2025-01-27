using Newtonsoft.Json;
using System.Collections.Generic;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SetFeedbackMessage(Dictionary<string, string> dictKeyValues, string pluginUuid)
        : IMessage
    {
        [JsonProperty("event")]
        public string Event => "setFeedback";

        [JsonProperty("context")]
        public string Context { get; private set; } = pluginUuid;

        [JsonProperty("payload")]
        public Dictionary<string, string> DictKeyValues { get; private set; } = dictKeyValues;
    }
}
