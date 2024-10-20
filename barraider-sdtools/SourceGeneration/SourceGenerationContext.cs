using System.Text.Json.Serialization;
using BarRaider.SdTools.Communication.Messages;
using BarRaider.SdTools.Communication.SDEvents;

namespace BarRaider.SdTools.SourceGeneration;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(IMessage))]
[JsonSerializable(typeof(BaseEvent))]
[JsonSerializable(typeof(StreamDeckInfo.StreamDeckInfo))]
[JsonSerializable(typeof(KeyDownEvent))]
[JsonSerializable(typeof(KeyUpEvent))]
[JsonSerializable(typeof(WillAppearEvent))]
[JsonSerializable(typeof(WillDisappearEvent))]
[JsonSerializable(typeof(TitleParametersDidChangeEvent))]
[JsonSerializable(typeof(DeviceDidConnectEvent))]
[JsonSerializable(typeof(DeviceDidDisconnectEvent))]
[JsonSerializable(typeof(ApplicationDidLaunchEvent))]
[JsonSerializable(typeof(ApplicationDidTerminateEvent))]
[JsonSerializable(typeof(SystemDidWakeUpEvent))]
[JsonSerializable(typeof(DidReceiveSettingsEvent))]
[JsonSerializable(typeof(DidReceiveGlobalSettingsEvent))]
[JsonSerializable(typeof(PropertyInspectorDidAppearEvent))]
[JsonSerializable(typeof(PropertyInspectorDidDisappearEvent))]
[JsonSerializable(typeof(SendToPluginEvent))]
[JsonSerializable(typeof(DialRotateEvent))]
[JsonSerializable(typeof(DialRotateEvent))]
[JsonSerializable(typeof(DialDownEvent))]
[JsonSerializable(typeof(DialUpEvent))]
[JsonSerializable(typeof(TouchpadPressEvent))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}