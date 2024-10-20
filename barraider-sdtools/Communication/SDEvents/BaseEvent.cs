using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using BarRaider.SdTools.SourceGeneration;
using BarRaider.SdTools.Tools;

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
        public const string TOUCHPAD_PRESS = "touchTap";
    }
    
    /// <summary>
    /// Base event that all the actual events derive from
    /// </summary>
    public abstract class BaseEvent
    {
        /// <summary>
        /// Provides the appropriate JsonTypeInfo for the given implementation of BaseEvent.
        /// </summary>
        /// <param name="eventType">A string representing a BaseEvent name.</param>
        /// <returns>A JsonTypeInfo instance for the given event.</returns>
        /// <exception cref="ArgumentException">Thrown if the provided argument does not correspond to an implementation
        /// of BaseEvent</exception>
        private static JsonTypeInfo BaseEventTypeInfo(string eventType) => eventType switch
        {
            EventTypes.KEY_DOWN => SourceGenerationContext.Default.KeyDownEvent,
            EventTypes.KEY_UP => SourceGenerationContext.Default.KeyUpEvent,

            EventTypes.WILL_APPEAR => SourceGenerationContext.Default.WillAppearEvent,
            EventTypes.WILL_DISAPPEAR => SourceGenerationContext.Default.WillDisappearEvent,

            EventTypes.TITLE_PARAMETERS_DID_CHANGE => SourceGenerationContext.Default.TitleParametersDidChangeEvent,

            EventTypes.DEVICE_DID_CONNECT => SourceGenerationContext.Default.DeviceDidConnectEvent,
            EventTypes.DEVICE_DID_DISCONNECT => SourceGenerationContext.Default.DeviceDidDisconnectEvent,

            EventTypes.APPLICATION_DID_LAUNCH => SourceGenerationContext.Default.ApplicationDidLaunchEvent,
            EventTypes.APPLICATION_DID_TERMINATE => SourceGenerationContext.Default.ApplicationDidTerminateEvent,

            EventTypes.SYSTEM_DID_WAKE_UP => SourceGenerationContext.Default.SystemDidWakeUpEvent,

            EventTypes.DID_RECEIVE_SETTINGS => SourceGenerationContext.Default.DidReceiveSettingsEvent,
            EventTypes.DID_RECEIVE_GLOBAL_SETTINGS => SourceGenerationContext.Default.DidReceiveGlobalSettingsEvent,

            EventTypes.PROPERTY_INSPECTOR_DID_APPEAR => SourceGenerationContext.Default.PropertyInspectorDidAppearEvent,
            EventTypes.PROPERTY_INSPECTOR_DID_DISAPPEAR => SourceGenerationContext.Default.PropertyInspectorDidDisappearEvent,

            EventTypes.SEND_TO_PLUGIN => SourceGenerationContext.Default.SendToPluginEvent,

            EventTypes.DIAL_ROTATE => SourceGenerationContext.Default.DialRotateEvent,
            EventTypes.DIAL_DOWN => SourceGenerationContext.Default.DialDownEvent,
            EventTypes.DIAL_UP => SourceGenerationContext.Default.DialUpEvent,
            EventTypes.TOUCHPAD_PRESS => SourceGenerationContext.Default.TouchpadPressEvent,
            EventTypes.DIAL_PRESS => SourceGenerationContext.Default.DialDownEvent, // Deprecated: Should be removed whenEvent stops getting sent by SD
            _ => throw new ArgumentException($"Type {eventType} is not supported or is unknown.")
        };


        /// <summary>
        /// Name of the event raised
        /// </summary>
        [JsonPropertyName("event")]
        public string Event { get; set; }

        internal static BaseEvent Parse(string json)
        {
            var jsonObject = JsonSerializer.SerializeToNode(json, SourceGenerationContext.Default.BaseEvent) as JsonObject;
            if (!jsonObject?.ContainsKey("event") ?? false) return null;
            
            string eventType = jsonObject?["event"]?.ToString() ?? string.Empty;

            try
            {
                // return as a specific subclass of BaseEvent cast as a BaseEvent instead of an abstract BaseEvent
                return JsonSerializer.Deserialize(json, BaseEventTypeInfo(eventType)) as BaseEvent;
            }
            catch(ArgumentException e)
            {
                Logger.Instance.LogMessage(TracingLevel.Error, $"BaseEvent parsing exception: {e}");
                return null;
            }
        }
    }
}
