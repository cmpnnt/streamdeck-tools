﻿using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SetGlobalSettingsMessage : IMessage
    {
        [JsonPropertyName("event")]
        public string Event { get { return "setGlobalSettings"; } }

        [JsonPropertyName("context")]
        public string Context { get; set; }

        [JsonPropertyName("payload")]
        public JsonObject Payload { get; set; }

        public SetGlobalSettingsMessage(JsonObject settings, string pluginUuid)
        {
            Context = pluginUuid;
            Payload = settings;
        }
    }
}
