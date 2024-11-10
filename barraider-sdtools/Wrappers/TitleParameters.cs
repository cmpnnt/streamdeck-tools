using Newtonsoft.Json;
using System;
using BarRaider.SdTools.Utilities;
using SkiaSharp;

namespace BarRaider.SdTools.Wrappers
{
    /// <summary>
    /// Enum for the alignment of the Title text on the key
    /// </summary>
    public enum TitleVerticalAlignment
    {
        /// <summary>
        /// Top Alignment
        /// </summary>
        Top,

        /// <summary>
        /// Middle/Center Alignment
        /// </summary>
        Middle,

        /// <summary>
        /// Bottom Alignment
        /// </summary>
        Bottom
    }
    
    /// <summary>
    /// Enum for the alignment of the Title text on the key
    /// </summary>
    public enum TitleHorizontalAlignment
    {
        /// <summary>
        /// Top Alignment
        /// </summary>
        Left,

        /// <summary>
        /// Middle/Center Alignment
        /// </summary>
        Middle,

        /// <summary>
        /// Bottom Alignment
        /// </summary>
        Right
    }

    /// <summary>
    /// Class holding all the Title Information set by a user in the Property Inspector
    /// </summary>
    public class TitleParameters
    {
        #region Private Members
        private const double POINTS_TO_PIXEL_CONVERT = 1.3;
        private const int DEFAULT_IMAGE_SIZE_FONT_SCALE = 3;
        private const string DEFAULT_FONT_FAMILY_NAME = "Verdana";
        #endregion

        /// <summary>
        /// Title color
        /// </summary>
        [JsonProperty("titleColor")]
        public SKColor TitleColor { get; private set; } = SKColors.White;
        
        /// <summary>
        /// Title stroke color
        /// </summary>
        [JsonIgnore]
        public SKColor TitleStrokeColor { get; set; }

        /// <summary>
        /// Title stroke thickness
        /// </summary>
        [JsonIgnore]
        public float TitleStrokeThickness { get; set; } = 1f;

        /// <summary>
        /// Font Size in Points
        /// </summary>
        [JsonProperty("fontSize")]
        public double FontSizeInPoints { get; private set; } = 10;

        /// <summary>
        /// Font size in Pixels
        /// </summary>
        [JsonIgnore]
        public double FontSizeInPixels => Math.Round(FontSizeInPoints * POINTS_TO_PIXEL_CONVERT);

        /// <summary>
        /// Font size Scaled to Image
        /// </summary>
        [JsonIgnore]
        public double FontSizeInPixelsScaledToDefaultImage => Math.Round(FontSizeInPixels * DEFAULT_IMAGE_SIZE_FONT_SCALE);

        /// <summary>
        /// Font family
        /// </summary>
        [JsonProperty("fontFamily")]
        public SKTypeface FontFamily { get; private set; } = SKTypeface.FromFamilyName(DEFAULT_FONT_FAMILY_NAME);

        /// <summary>
        /// Font style
        /// </summary>
        [JsonProperty("fontStyle")]
        public SKFontStyle FontStyle { get; private set; } = SKFontStyle.Bold;

        /// <summary>
        /// Should title be shown
        /// </summary>
        [JsonProperty("showTitle")]
        public bool ShowTitle { get; private set; }

        /// <summary>
        /// Vertical alignment of the title text on the key
        /// </summary>
        [JsonProperty("titleAlignment")]
        public TitleVerticalAlignment VerticalAlignment { get; private set; }
        
        /// <summary>
        /// Horizontal alignment of the title text on the key
        /// </summary>
        [JsonIgnore]
        public TitleHorizontalAlignment HorizontalAlignment { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fontFamily"></param>
        /// <param name="fontStyle"></param>
        /// <param name="fontSize"></param>
        /// <param name="titleColor"></param>
        /// <param name="showTitle"></param>
        /// <param name="verticalAlignment"></param>
        /// <param name="horizontalAlignment"></param>
        public TitleParameters(
            SKTypeface fontFamily,
            SKFontStyle fontStyle,
            double fontSize,
            SKColor titleColor,
            bool showTitle = true,
            TitleVerticalAlignment verticalAlignment = TitleVerticalAlignment.Middle,
            TitleHorizontalAlignment horizontalAlignment = TitleHorizontalAlignment.Middle)
        {
            FontFamily = fontFamily;
            FontStyle = fontStyle;
            FontSizeInPoints = fontSize;
            TitleColor = titleColor;
            ShowTitle = showTitle;
            VerticalAlignment = verticalAlignment;
            HorizontalAlignment = horizontalAlignment;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fontFamily"></param>
        /// <param name="fontSize"></param>
        /// <param name="fontStyle"></param>
        /// <param name="fontUnderline"></param>
        /// <param name="showTitle"></param>
        /// <param name="titleAlignment"></param>
        /// <param name="titleColor"></param>
        public TitleParameters(string fontFamily, uint fontSize, string fontStyle, bool fontUnderline, bool showTitle, string titleAlignment, string titleColor)
        {
            ParsePayload(fontFamily, fontSize, fontStyle, fontUnderline, showTitle, titleAlignment, titleColor);
        }

        private void ParsePayload(string fontFamily, uint fontSize, string fontStyle, bool fontUnderline, bool showTitle, string titleAlignment, string titleColor)
        {
            try
            {
                ShowTitle = showTitle;

                // Color
                if (!string.IsNullOrEmpty(titleColor)) TitleColor = SKColor.Parse(titleColor);
                if (!string.IsNullOrEmpty(fontFamily)) FontFamily = SKTypeface.FromFamilyName(fontFamily);
                
                FontSizeInPoints = fontSize;
                if (!string.IsNullOrEmpty(fontStyle))
                {
                    switch (fontStyle.ToLowerInvariant())
                    {
                        case "regular":
                            FontStyle = SKFontStyle.Normal;
                            break;
                        case "bold":
                            FontStyle = SKFontStyle.Bold;
                            break;
                        case "italic":
                            FontStyle = SKFontStyle.Italic;
                            break;
                        case "bold italic":
                            FontStyle = SKFontStyle.BoldItalic;
                            break;
                        default:
                            Logger.Instance.LogMessage(TracingLevel.Warn, $"{GetType()} Cannot parse Font Style: {fontStyle}");
                            break;
                    }
                }
                
                // TODO: remove underline from the parameters of this method
                if (fontUnderline) FontStyle = SKFontStyle.Normal;
                
                if (!string.IsNullOrEmpty(titleAlignment))
                {
                    VerticalAlignment = titleAlignment.ToLowerInvariant() switch
                    {
                        "top" => TitleVerticalAlignment.Top,
                        "bottom" => TitleVerticalAlignment.Bottom,
                        "middle" => TitleVerticalAlignment.Middle,
                        _ => VerticalAlignment
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.Error, $"TitleParameters failed to parse payload {ex}");
            }
        }
    }
}
