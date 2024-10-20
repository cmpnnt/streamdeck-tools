using System.Text.Json.Serialization;

namespace BarRaider.SdTools.StreamDeckInfo;

public record StreamDeckColors
{
    [JsonPropertyName("buttonMouseOverBackgroundColor")]
    public string ButtonMouseOverBackgroundColor { get; set; }
    
    [JsonPropertyName("buttonPressedBackgroundColor")]
    public string ButtonPressedBackgroundColor { get; set; }
    
    [JsonPropertyName("buttonPressedBorderColor")]
    public string ButtonPressedBorderColor { get; set; }
    
    [JsonPropertyName("buttonPressedTextColor")]
    public string ButtonPressedTextColor { get; set; }
    
    [JsonPropertyName("highlightColor")]
    public string HighlightColor { get; set; }
}