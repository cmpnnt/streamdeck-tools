using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using BarRaider.SdTools.Wrappers;

namespace BarRaider.SdTools.Utilities
{
    /// <summary>
    /// Extension methods for various objects
    /// </summary>
    public static class ExtensionMethods
    {
        #region Coordinates
        /// <summary>
        /// Checks if too KeyCoordinates match to the same key
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="secondCoordinates"></param>
        /// <returns></returns>
        public static bool IsCoordinatesSame(this KeyCoordinates coordinates, KeyCoordinates secondCoordinates)
        {
            if (secondCoordinates == null) return false;
            return coordinates.Row == secondCoordinates.Row && coordinates.Column == secondCoordinates.Column;
        }
        #endregion

        #region Brushes/Colors
        /// <summary>
        /// Shows Color In Hex Format
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ToHex(this Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        /// <summary>
        /// Shows Color in Hex format
        /// </summary>
        /// <param name="brush"></param>
        /// <returns></returns>
        public static string ToHex(this Brush brush)
        {
            if (brush is SolidBrush solidBrush)
            {
                return solidBrush.Color.ToHex();
            }
            return null;
        }
        #endregion

        #region Image/Graphics
        /// <summary>
        /// Converts an Image into a Byte Array
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this Image image)
        {
            using var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Bmp);
            return ms.ToArray();
        }

        /// <summary>
        /// Convert a in-memory image object to Base64 format. Set the addHeaderPrefix to true, if this is sent to the SendImageAsync function
        /// </summary>
        /// <param name="image"></param>
        /// <param name="addHeaderPrefix"></param>
        /// <returns></returns>
        public static string ToBase64(this Image image, bool addHeaderPrefix)
        {
            return Tools.ImageToBase64(image, addHeaderPrefix);
        }

        /// <summary>
        /// Draws a string on a Graphics object and returns the ending Y position of the string
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="brush"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static float DrawAndMeasureString(this Graphics graphics, string text, Font font, Brush brush, PointF position)
        {
            SizeF stringSize = graphics.MeasureString(text, font);
            graphics.DrawString(text, font, brush, position);

            return position.Y + stringSize.Height;
        }

        /// <summary>
        /// Returns the center X position of a string, given the image's max Width and Font information
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="text"></param>
        /// <param name="imageWidth"></param>
        /// <param name="font"></param>
        /// /// <param name="textFitsImage">True/False - Does text fit image? False if text overflows</param>
        /// <param name="minIndentation"></param>
        /// 
        /// <returns></returns>
        public static float GetTextCenter(this Graphics graphics, string text, int imageWidth, Font font, out bool textFitsImage, int minIndentation = 0)
        {
            SizeF stringSize = graphics.MeasureString(text, font);
            float stringWidth = minIndentation;
            textFitsImage = false;
            
            if (!(stringSize.Width < imageWidth)) return stringWidth;
            
            textFitsImage = true;
            stringWidth = Math.Abs((imageWidth - stringSize.Width)) / 2;
            
            return stringWidth;
        }

        /// <summary>
        /// Returns the center X position of a string, given the image's max Width and Font information
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="text"></param>
        /// <param name="imageWidth"></param>
        /// <param name="font"></param>
        /// <param name="minIndentation"></param>
        /// 
        /// <returns></returns>
        public static float GetTextCenter(this Graphics graphics, string text, int imageWidth, Font font, int minIndentation = 0)
        {
            return graphics.GetTextCenter(text, imageWidth, font, out _, minIndentation);
        }

        /// <summary>
        /// Returns the highest size of the given font in which the text fits the image
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="text"></param>
        /// <param name="imageWidth"></param>
        /// <param name="font"></param>
        /// /// <param name="minimalFontSize"></param>
        /// <returns></returns>
        public static float GetFontSizeWhereTextFitsImage(this Graphics graphics, string text, int imageWidth, Font font, int minimalFontSize = 6)
        {
            bool textFitsImage;
            float size = font.Size;
            var variableFont = new Font(font.Name, size, font.Style, GraphicsUnit.Pixel);
            do
            {
                graphics.GetTextCenter(text, imageWidth, variableFont, out textFitsImage);
                if (textFitsImage) continue;
                
                variableFont.Dispose();
                size -= 0.5f;
                variableFont = new Font(font.Name, size, font.Style, GraphicsUnit.Pixel);
            }
            while (!textFitsImage && size > minimalFontSize);

            variableFont.Dispose();
            return size;
        }

        /// <summary>
        /// Adds a text path to an existing Graphics object. Uses TitleParameters to emulate the Text settings in the Property Inspector
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="titleParameters"></param>
        /// <param name="imageHeight"></param>
        /// <param name="imageWidth"></param>
        /// <param name="text"></param>
        /// <param name="pixelsAlignment"></param>
        public static void AddTextPath(this Graphics graphics, TitleParameters titleParameters, int imageHeight, int imageWidth, string text, int pixelsAlignment = 15)
        {
            AddTextPath(graphics, titleParameters, imageHeight, imageWidth, text, Color.Black, 1, pixelsAlignment);
        }

        /// <summary>
        /// Adds a text path to an existing Graphics object. Uses TitleParameters to emulate the Text settings in the Property Inspector
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="titleParameters"></param>
        /// <param name="imageHeight"></param>
        /// <param name="imageWidth"></param>
        /// <param name="text"></param>
        /// <param name="strokeColor"></param>
        /// <param name="strokeThickness"></param>
        /// <param name="pixelsAlignment"></param>
        public static void AddTextPath(this Graphics graphics, TitleParameters titleParameters, int imageHeight, int imageWidth, string text, Color strokeColor, float strokeThickness, int pixelsAlignment = 15)
        {
            try
            {
                if (titleParameters == null)
                {
                    Logger.Instance.LogMessage(TracingLevel.Error, $"AddTextPath: titleParameters is null");
                    return;
                }

                var font = new Font(titleParameters.FontFamily, (float)titleParameters.FontSizeInPixelsScaledToDefaultImage, titleParameters.FontStyle, GraphicsUnit.Pixel);
                Color color = titleParameters.TitleColor;
                graphics.PageUnit = GraphicsUnit.Pixel;
                float ratio = graphics.DpiY / imageWidth;
                SizeF stringSize = graphics.MeasureString(text, font);
                float textWidth = stringSize.Width * (1 - ratio);
                float textHeight = stringSize.Height * (1 - ratio);
                var stringWidth = 0;
                
                if (textWidth < imageWidth)
                {
                    stringWidth = (int)(Math.Abs((imageWidth - textWidth)) / 2) - pixelsAlignment;
                }

                int stringHeight = titleParameters.VerticalAlignment switch
                {
                    TitleVerticalAlignment.Middle => (imageHeight / 2) - pixelsAlignment,
                    TitleVerticalAlignment.Bottom => (int)(Math.Abs((imageHeight - textHeight)) - pixelsAlignment),
                    _ => pixelsAlignment
                };

                var stroke = new Pen(strokeColor, strokeThickness);
                var gpath = new GraphicsPath();
                gpath.AddString(text,
                                    font.FontFamily,
                                    (int)font.Style,
                                    graphics.DpiY * font.SizeInPoints / imageWidth,
                                    new Point(stringWidth, stringHeight),
                                    new StringFormat());
                graphics.DrawPath(stroke, gpath);
                graphics.FillPath(new SolidBrush(color), gpath);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.Error, $"AddTextPath Exception {ex}");
            }
        }
        #endregion

        #region String
        /// <summary>
        /// /// Truncates a string to the first maxSize characters. If maxSize is less than string length, original string will be returned
        /// </summary>
        /// <param name="str">String</param>
        /// <param name="maxSize">Max size for string</param>
        /// <returns></returns>
        public static string Truncate(this string str, int maxSize)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            return maxSize < 1 ? str : str[..Math.Min(Math.Max(0, maxSize), str.Length)];
        }

        /// <summary>
        /// Adds line breaks (\n) to the text to make sure it fits the key when using SetTitleAsync()
        /// </summary>
        /// <param name="str"></param>
        /// <param name="titleParameters"></param>
        /// <param name="leftPaddingPixels"></param>
        /// <param name="rightPaddingPixels"></param>
        /// <param name="imageWidthPixels"></param>
        /// <returns></returns>
        public static string SplitToFitKey(this string str, TitleParameters titleParameters, int leftPaddingPixels = 3, int rightPaddingPixels = 3, int imageWidthPixels = 72)
        {
            try
            {
                if (titleParameters == null) return str;
                
                int padding = leftPaddingPixels + rightPaddingPixels;
                var font = new Font(titleParameters.FontFamily, (float)titleParameters.FontSizeInPoints, titleParameters.FontStyle, GraphicsUnit.Pixel);
                var finalString = new StringBuilder();
                var currentLine = new StringBuilder();

                using (var img = new Bitmap(imageWidthPixels, imageWidthPixels))
                {
                    using (Graphics graphics = Graphics.FromImage(img))
                    {
                        foreach (char c in str)
                        {
                            currentLine.Append(c);
                            SizeF currentLineSize = graphics.MeasureString(currentLine.ToString(), font);
                            if (currentLineSize.Width <= img.Width - padding)
                            {
                                finalString.Append(c);
                            }
                            else // Overflow
                            {
                                finalString.Append("\n" + c);
                                currentLine = new StringBuilder(c.ToString());
                            }
                        }
                    }
                }

                return finalString.ToString();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.Error, $"SplitStringToFit Exception: {ex}");
                return str;
            }
        }
        #endregion
    }
}
