using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// List of all supported event typs
    /// </summary>
    internal static class EventTypes
    {
        public const string KEY_DOWN = "keyDown";
        public const string KEY_UP = "keyUp";
        public const string WILL_APPEAR = "willAppear";
        public const string WILL_DISAPPEAR = "willDisappear";
        public const string TITLE_PARAMETERS_DID_CHANGE = "titleParametersDidChange";
        public const string DEVICE_DID_CONNECT = "deviceDidConnect";
        public const string DEVICE_DID_DISCONNECT = "deviceDidDisconnect";
        public const string APPLICATION_DID_LAUNCH = "applicationDidLaunch";
        public const string APPLICATION_DID_TERMINATE = "applicationDidTerminate";
        public const string SYSTEM_DID_WAKE_UP = "systemDidWakeUp";
        public const string DID_RECEIVE_SETTINGS = "didReceiveSettings";
        public const string DID_RECEIVE_GLOBAL_SETTINGS = "didReceiveGlobalSettings";
        public const string PROPERTY_INSPECTOR_DID_APPEAR = "propertyInspectorDidAppear";
        public const string PROPERTY_INSPECTOR_DID_DISAPPEAR = "propertyInspectorDidDisappear";
        public const string SEND_TO_PLUGIN = "sendToPlugin";
        public const string DIAL_ROTATE = "dialRotate";
        public const string DIAL_PRESS = "dialPress";
        public const string DIAL_DOWN = "dialDown";
        public const string DIAL_UP = "dialUp";
        public const string TOUCHTAP = "touchTap";
    }

    /// <summary>
    /// Base event that all the actual events derive from
    /// </summary>
    public abstract class BaseEvent
    {
        private static readonly Dictionary<string, Type> EventsMap = new()
        {
            { EventTypes.KEY_DOWN, typeof(KeyDownEvent) },
            { EventTypes.KEY_UP, typeof(KeyUpEvent) },

            { EventTypes.WILL_APPEAR, typeof(WillAppearEvent) },
            { EventTypes.WILL_DISAPPEAR, typeof(WillDisappearEvent) },

            { EventTypes.TITLE_PARAMETERS_DID_CHANGE, typeof(TitleParametersDidChangeEvent) },

            { EventTypes.DEVICE_DID_CONNECT, typeof(DeviceDidConnectEvent) },
            { EventTypes.DEVICE_DID_DISCONNECT, typeof(DeviceDidDisconnectEvent) },

            { EventTypes.APPLICATION_DID_LAUNCH, typeof(ApplicationDidLaunchEvent) },
            { EventTypes.APPLICATION_DID_TERMINATE, typeof(ApplicationDidTerminateEvent) },

            { EventTypes.SYSTEM_DID_WAKE_UP, typeof(SystemDidWakeUpEvent) },

            { EventTypes.DID_RECEIVE_SETTINGS, typeof(DidReceiveSettingsEvent) },
            { EventTypes.DID_RECEIVE_GLOBAL_SETTINGS, typeof(DidReceiveGlobalSettingsEvent) },

            { EventTypes.PROPERTY_INSPECTOR_DID_APPEAR, typeof(PropertyInspectorDidAppearEvent) },
            { EventTypes.PROPERTY_INSPECTOR_DID_DISAPPEAR, typeof(PropertyInspectorDidDisappearEvent) },

            { EventTypes.SEND_TO_PLUGIN, typeof(SendToPluginEvent) },

            { EventTypes.DIAL_ROTATE, typeof(DialRotateEvent) },
            { EventTypes.DIAL_DOWN, typeof(DialDownEvent) },
            { EventTypes.DIAL_UP, typeof(DialUpEvent) },
            { EventTypes.TOUCHTAP, typeof(TouchTapEvent) },
            { EventTypes.DIAL_PRESS, typeof(DialDownEvent) }, // Deprecated: Should be removed when event stops getting sent by SD
        };

        /// <summary>
        /// Name of the event raised
        /// </summary>
        [JsonProperty("event")]
        public string Event { get; set; }

        internal static BaseEvent Parse(string json)
        {
            JObject jsonObject = JObject.Parse(json);
            if (!jsonObject.ContainsKey("event"))
            {
                return null;
            }

            var eventType = jsonObject["event"].ToString();
            if (!EventsMap.TryGetValue(eventType, out Type value)) return null;
            
            return JsonConvert.DeserializeObject(json, value) as BaseEvent;
        }
    }
}
