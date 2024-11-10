using System;
using System.Text;
using BarRaider.SdTools.Wrappers;
using SkiaSharp;

namespace BarRaider.SdTools.Utilities
{
    /// <summary>
    /// Library of tools used to manipulate graphics
    /// </summary>
    public static class GraphicsTools
    {
        /// <summary>
        /// Return a Color object based on the hex value
        /// </summary>
        /// <param name="hexColor"></param>
        /// <returns></returns>
        public static SKColor ColorFromHex(string hexColor)
        {
            return SKColor.Parse(hexColor);
        }

        /// <summary>
        /// Generates multiple shades based on an initial color, and number of stages/shades you want
        /// </summary>
        /// <param name="initialColor"></param>
        /// <param name="currentShade"></param>
        /// <param name="totalAmountOfShades"></param>
        /// <returns></returns>
        public static SKColor GenerateColorShades(string initialColor, int currentShade, int totalAmountOfShades)
        {
            SKColor color = ColorFromHex(initialColor);
            byte a = color.Alpha;
            byte r = color.Red;
            byte g = color.Green;
            byte b = color.Blue;

            // Try and increase the color in the last stage;
            if (currentShade == totalAmountOfShades - 1)
            {
                currentShade = 1;
            }

            for (var idx = 0; idx < currentShade; idx++)
            {
                r /= 2;
                g /= 2;
                b /= 2;
            }

            return new SKColor(r, g, b, a);
        }

        /// <summary>
        /// Resizes an image while scaling
        /// </summary>
        /// <param name="original"></param>
        /// <param name="newWidth"></param>
        /// <returns></returns>
        public static SKBitmap ResizeImage(SKBitmap original, int newWidth)
        {
            // Calculate the new height to maintain the aspect ratio
            float aspectRatio = (float)original.Height / original.Width;
            var newHeight = (int)(newWidth * aspectRatio);

            // Create a new bitmap with the desired dimensions
            var resizedImage = new SKBitmap(newWidth, newHeight);

            using var canvas = new SKCanvas(resizedImage);
            // Draw the original image onto the new bitmap, scaling it to the new size
            canvas.DrawBitmap(original, new SKRect(0, 0, newWidth, newHeight));

            return resizedImage;
        }


        /// <summary>
        /// Extract a part of an image (aka crop)
        /// </summary>
        /// <param name="original"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static SKBitmap CropImage(SKBitmap original, int startX, int startY, int width, int height)
        {
            var cropRect = new SKRectI(startX, startY, width, height);
            
            // Ensure the crop rectangle is within the bounds of the original image
            cropRect.Intersect(new SKRectI(0, 0, original.Width, original.Height));

            // Create a new bitmap to hold the cropped image
            var croppedImage = new SKBitmap(cropRect.Width, cropRect.Height);

            using var canvas = new SKCanvas(croppedImage);
            // Draw the specified rectangle from the original image onto the new bitmap
            canvas.DrawBitmap(original, cropRect, new SKRect(0, 0, cropRect.Width, cropRect.Height));

            return croppedImage;
        }
        
        /// <summary>
        /// Creates a new image with different opacity
        /// </summary>
        /// <param name="original"></param>
        /// <param name="opacity"></param>
        /// <returns></returns>
        public static SKBitmap ChangeOpacity(SKBitmap original, byte opacity)
        {
            try
            {
                // Create a new bitmap to hold the image with changed opacity
                var newBitmap = new SKBitmap(original.Width, original.Height);
                using var canvas = new SKCanvas(newBitmap);
               
                // Create a paint object with the desired opacity
                var paint = new SKPaint();
                paint.Color = new SKColor(255, 255, 255, opacity);
                paint.IsAntialias = true;

                // Draw the original image onto the new bitmap using the paint object
                canvas.Clear();
                canvas.DrawBitmap(original, new SKRect(0, 0, original.Width, original.Height), paint);

                return newBitmap;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.Error, $"SetImageOpacity exception {ex}");
                return null;
            }
        }

        /// <summary>
        /// Adds line breaks ('\n') to the string every time the text would overflow the image
        /// </summary>
        /// <param name="str"></param>
        /// <param name="titleParameters"></param>
        /// <param name="leftPaddingPixels"></param>
        /// <param name="rightPaddingPixels"></param>
        /// <param name="imageWidthPixels"></param>
        /// <returns></returns>
        public static string WrapStringToFitImage(
            string str,
            TitleParameters titleParameters,
            int leftPaddingPixels = 5,
            int rightPaddingPixels = 5,
            int imageWidthPixels = 72)
        {
            try
            {
                if (titleParameters == null)
                {
                    return str;
                }

                int padding = leftPaddingPixels + rightPaddingPixels;
                var finalString = new StringBuilder();
                var currentLine = new StringBuilder();

                using (var paint = new SKPaint())
                {
                    paint.Typeface = SKTypeface.FromFamilyName(titleParameters.FontFamily.FamilyName);
                    paint.TextSize = (float)titleParameters.FontSizeInPixels;
                    foreach (char t in str)
                    {
                        currentLine.Append(t);
                        float currentLineSize = paint.MeasureText(currentLine.ToString());
                        if (currentLineSize <= imageWidthPixels - padding)
                        {
                            finalString.Append(t);
                        }
                        else // Overflow
                        {
                            finalString.Append("\n" + t);
                            currentLine = new StringBuilder(t.ToString());
                        }
                    }
                }

                return finalString.ToString();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.Error, $"WrapStringToFitImage Exception: {ex}");
                return str;
            }
        }
    }
}
