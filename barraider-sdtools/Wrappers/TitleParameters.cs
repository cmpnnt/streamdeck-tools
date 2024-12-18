﻿using System;
using System.Drawing;
using System.Text.Json.Serialization;
using BarRaider.SdTools.Tools;

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
        /// Title Color
        /// </summary>
        [JsonPropertyName("titleColor")]
        public Color TitleColor { get; set; } = Color.White;

        /// <summary>
        /// Font Size in Points
        /// </summary>
        [JsonPropertyName("fontSize")]
        public double FontSizeInPoints { get; set; } = 10;

        /// <summary>
        /// Font Size in Pixels
        /// </summary>
        [JsonIgnore]
        public double FontSizeInPixels => Math.Round(FontSizeInPoints * POINTS_TO_PIXEL_CONVERT);

        /// <summary>
        /// Font Size Scaled to Image
        /// </summary>
        [JsonIgnore]
        public double FontSizeInPixelsScaledToDefaultImage => Math.Round(FontSizeInPixels * DEFAULT_IMAGE_SIZE_FONT_SCALE);

        /// <summary>
        /// Font Family
        /// </summary>
        [JsonPropertyName("fontFamily")]
        public FontFamily FontFamily { get; set; } = new FontFamily(DEFAULT_FONT_FAMILY_NAME);

        /// <summary>
        /// Font Style
        /// </summary>
        [JsonPropertyName("fontStyle")]
        public FontStyle FontStyle { get; set; } = FontStyle.Bold;

        /// <summary>
        /// Should Title be shown
        /// </summary>
        [JsonPropertyName("showTitle")]
        public bool ShowTitle { get; set; }

        /// <summary>
        /// Alignment position of the Title text on the key
        /// </summary>
        [JsonPropertyName("titleAlignment")]
        public TitleVerticalAlignment VerticalAlignment { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fontFamily"></param>
        /// <param name="fontStyle"></param>
        /// <param name="fontSize"></param>
        /// <param name="titleColor"></param>
        /// <param name="showTitle"></param>
        /// <param name="verticalAlignment"></param>
        public TitleParameters(FontFamily fontFamily, FontStyle fontStyle, double fontSize, Color titleColor, bool showTitle, TitleVerticalAlignment verticalAlignment)
        {
            FontFamily = fontFamily;
            FontStyle = fontStyle;
            FontSizeInPoints = fontSize;
            TitleColor = titleColor;
            ShowTitle = showTitle;
            VerticalAlignment = verticalAlignment;
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
                if (!string.IsNullOrEmpty(titleColor))
                {
                    TitleColor = ColorTranslator.FromHtml(titleColor);
                }

                if (!string.IsNullOrEmpty(fontFamily))
                {
                    FontFamily = new FontFamily(fontFamily);
                }

                FontSizeInPoints = fontSize;
                if (!string.IsNullOrEmpty(fontStyle))
                {
                    switch (fontStyle.ToLowerInvariant())
                    {
                        case "regular":
                            FontStyle = FontStyle.Regular;
                            break;
                        case "bold":
                            FontStyle = FontStyle.Bold;
                            break;
                        case "italic":
                            FontStyle = FontStyle.Italic;
                            break;
                        case "bold italic":
                            FontStyle = FontStyle.Bold | FontStyle.Italic;
                            break;
                        default:
                            Logger.Instance.LogMessage(TracingLevel.Warn, $"{GetType()} Cannot parse Font Style: {fontStyle}");
                            break;
                    }
                }
                if (fontUnderline)
                {
                    FontStyle |= FontStyle.Underline;
                }

                if (!string.IsNullOrEmpty(titleAlignment))
                {
                    switch (titleAlignment.ToLowerInvariant())
                    {
                        case "top":
                            VerticalAlignment = TitleVerticalAlignment.Top;
                            break;
                        case "bottom":
                            VerticalAlignment = TitleVerticalAlignment.Bottom;
                            break;
                        case "middle":
                            VerticalAlignment = TitleVerticalAlignment.Middle;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.Error, $"TitleParameters failed to parse payload {ex}");
            }
        }
    }
}
