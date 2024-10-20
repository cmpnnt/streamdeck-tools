using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SetFeedbackMessage : IMessage
    {
        [JsonPropertyName("event")]
        public string Event { get { return "setFeedback"; } }

        [JsonPropertyName("context")]
        public string Context { get; set; }

        [JsonPropertyName("payload")]
        public Dictionary<string, string> DictKeyValues { get; set; }

        public SetFeedbackMessage(Dictionary<string, string> dictKeyValues, string pluginUuid)
        {
            Context = pluginUuid;
            DictKeyValues = dictKeyValues;
        }
    }
}
