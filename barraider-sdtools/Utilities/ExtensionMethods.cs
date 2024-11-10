using System;
using System.Text;
using BarRaider.SdTools.Wrappers;
using SkiaSharp;

namespace BarRaider.SdTools.Utilities
{
    /// <summary>
    /// Extension methods for various objects
    /// </summary>
    public static class ExtensionMethods
    {
        #region Coordinates
        /// <summary>
        /// Checks if two KeyCoordinates match to the same key
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
        public static string ToHex(this SKColor color)
        {
            return $"#{color.Red:X2}{color.Green:X2}{color.Blue:X2}";
        }

        /// <summary>
        /// Shows Color in Hex format
        /// </summary>
        /// <param name="brush"></param>
        /// <returns></returns>
        public static string ToHex(this SKPaint brush)
        {
            return brush.Color.ToHex();
        }
        #endregion

        #region Image/Graphics
        /// <summary>
        /// Converts an Image into a Byte Array
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this SKImage image)
        {
            using SKBitmap bitmap = SKBitmap.FromImage(image);
            return bitmap.Bytes;
        }

        /// <summary>
        /// Convert an in-memory SKImage to Base64 format. Set the addHeaderPrefix to true, if this is sent to the SendImageAsync function
        /// </summary>
        /// <param name="image"></param>
        /// <param name="addHeaderPrefix"></param>
        /// <returns></returns>
        public static string ToBase64(this SKImage image, bool addHeaderPrefix)
        {
            return Tools.ImageToBase64(image, addHeaderPrefix);
        }
        
        /// <summary>
        /// Convert an in-memory SKImage to Base64 format. Set the addHeaderPrefix to true, if this is sent to the SendImageAsync function
        /// </summary>
        /// <param name="image"></param>
        /// <param name="addHeaderPrefix"></param>
        /// <returns></returns>
        public static string ToBase64(this SKBitmap image, bool addHeaderPrefix)
        {
            return Tools.ImageToBase64(image, addHeaderPrefix);
        }

        /// <summary>
        /// Draws a string on a Canvas object and returns the ending Y position of the string
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="brush"></param>
        /// <param name="text"></param>
        /// <param name="position"></param>
        /// <param name="horizontal"></param>
        /// <returns></returns>
        public static float DrawAndMeasureString(this SKCanvas canvas, SKPaint brush, string text, SKPoint position, bool horizontal = true)
        {
            SKRect bounds = canvas.LocalClipBounds;
            float stringSize = brush.MeasureText(text, ref bounds);
            canvas.DrawText(text, position, brush);
            //TODO: These measurements might not be accurate.
            //SEE: https://stackoverflow.com/questions/69907690/using-c-sharp-to-measure-the-width-of-a-string-in-pixels-in-a-cross-platform-way
            
            return (horizontal ? position.X : position.Y) + stringSize;
        }
        
        /// <summary>
        /// Draws a string on a Graphics object and returns the ending Y position of the string
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="brush"></param>
        /// <param name="text"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static float DrawAndMeasureStringWidth(this SKCanvas canvas, SKPaint brush, string text, SKPoint position)
        {
            return DrawAndMeasureString(canvas, brush, text, position);
        }
        
        /// <summary>
        /// Draws a string on a Graphics object and returns the ending Y position of the string
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="brush"></param>
        /// <param name="text"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static float DrawAndMeasureStringHeight(this SKCanvas canvas, SKPaint brush, string text, SKPoint position)
        {
           return DrawAndMeasureString(canvas, brush, text, position, false);
        }

        /// <summary>
        /// Returns the center X position of a string, given the image's max Width and Font information
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="brush"></param>
        /// <param name="text"></param>
        /// <param name="imageWidth"></param>
        /// <param name="textFitsImage">True/False - Does text fit image? False if text overflows</param>
        /// <param name="minIndentation"></param>
        /// 
        /// <returns></returns>
        public static float GetTextCenter(this SKCanvas canvas, SKPaint brush, string text, int imageWidth, out bool textFitsImage, int minIndentation = 0)
        {
            float stringSize = brush.MeasureText(text);
            float stringWidth = minIndentation;
            textFitsImage = false;
            
            if (!(stringSize < imageWidth)) return stringWidth;
            
            textFitsImage = true;
            stringWidth = Math.Abs((imageWidth - stringSize)) / 2;
            
            return stringWidth;
        }

        /// <summary>
        /// Returns the center X position of a string, given the image's max Width and Font information
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="brush"></param>
        /// <param name="text"></param>
        /// <param name="imageWidth"></param>
        /// <param name="minIndentation"></param>
        /// 
        /// <returns></returns>
        public static float GetTextCenter(this SKCanvas canvas, SKPaint brush, string text, int imageWidth, int minIndentation = 0)
        {
            return canvas.GetTextCenter(brush, text, imageWidth, out _, minIndentation);
        }

        /// <summary>
        /// Returns the highest size of the given font in which the text fits the image
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="text"></param>
        /// <param name="imageWidth"></param>
        /// <param name="font"></param>
        /// <param name="minimumFontSize"></param>
        /// <returns></returns>
        public static float GetFontSizeWhereTextFitsImage(this SKCanvas canvas, string text, int imageWidth, SKFont font, int minimumFontSize = 6)
        {
            bool textFitsImage;
            float size = font.Size;
            var variableFont = new SKFont(font.Typeface, size);

            var brush = new SKPaint
            {
                IsAntialias = true,
                Color = SKColors.Black,
            };
            
            do
            {
                canvas.GetTextCenter(brush, text, imageWidth, out textFitsImage, minimumFontSize);
                if (textFitsImage) continue;
                
                variableFont.Dispose();
                size -= 0.5f;
                variableFont = new SKFont(font.Typeface, size);
            }
            while (!textFitsImage && size > minimumFontSize);

            variableFont.Dispose();
            return size;
        }

        /// <summary>
        /// Adds a text path to an existing Graphics object. Uses TitleParameters to emulate the Text settings in the Property Inspector
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="titleParameters"></param>
        /// <param name="image"></param>
        /// <param name="text"></param>
        public static void AddTextPath(this SKCanvas canvas, TitleParameters titleParameters, SKBitmap image, string text)
        {
            try
            {
                if (titleParameters == null)
                {
                    Logger.Instance.LogMessage(TracingLevel.Error, $"AddTextPath: titleParameters is null");
                    return;
                }
                
                using var font = new SKFont(titleParameters.FontFamily, (float)titleParameters.FontSizeInPixelsScaledToDefaultImage);
                using var fill = new SKPaint(font);
                fill.TextSize = (float)titleParameters.FontSizeInPixels;
                fill.IsAntialias = true;
                fill.Color = titleParameters.TitleColor;
                fill.Style = titleParameters.TitleStrokeColor == default ? SKPaintStyle.StrokeAndFill : SKPaintStyle.Fill;
                fill.TextAlign = SKTextAlign.Center;
                
                float textHeight = fill.TextSize;
               
                double stringHeight = titleParameters.VerticalAlignment switch
                {
                    // ReSharper disable once PossibleLossOfFraction
                    TitleVerticalAlignment.Middle => (image.Height / 2) + titleParameters.FontSizeInPixels,
                    TitleVerticalAlignment.Bottom => (int)(Math.Abs((image.Height - textHeight)) + titleParameters.FontSizeInPixels),
                    _ => titleParameters.FontSizeInPixels
                };
                
                canvas.DrawText(text, (float)image.Width/2, (float)stringHeight, font, fill);

                if (titleParameters.TitleStrokeColor == default) return;
                
                using var stroke = new SKPaint(font);
                stroke.TextSize = (float)titleParameters.FontSizeInPixels;
                stroke.IsAntialias = true;
                stroke.Color = titleParameters.TitleStrokeColor;
                stroke.Style = SKPaintStyle.Stroke;
                stroke.TextAlign = SKTextAlign.Center;
                stroke.StrokeWidth = titleParameters.TitleStrokeThickness;
                canvas.DrawText(text, (float)image.Width/2, (float)stringHeight, font, stroke);
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
                var finalString = new StringBuilder();
                var currentLine = new StringBuilder();
                
                using (var paint = new SKPaint())
                {
                    paint.Typeface = SKTypeface.FromFamilyName(titleParameters.FontFamily.FamilyName);
                    paint.TextSize = (float)titleParameters.FontSizeInPixels;
                    foreach (char c in str)
                    {
                        currentLine.Append(c);
                        float currentLineSize = paint.MeasureText(currentLine.ToString());
                        if (currentLineSize <= imageWidthPixels - padding)
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
