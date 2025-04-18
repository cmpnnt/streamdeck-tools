﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using BarRaider.SdTools.Attributes;
using BarRaider.SdTools.StreamDeckInfo;
using BarRaider.SdTools.Wrappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SkiaSharp;

namespace BarRaider.SdTools.Utilities
{
    /// <summary>
    /// Set of common utilities used by various plugins
    /// Currently the class mostly focuses on image-related functions that will be passed to the StreamDeck key
    /// </summary>
    public static class Tools
    {
        private const string HEADER_PREFIX = "data:image/png;base64,";
        private const int CLASSIC_KEY_DEFAULT_HEIGHT = 72;
        private const int CLASSIC_KEY_DEFAULT_WIDTH = 72;
        private const int PLUS_KEY_DEFAULT_HEIGHT = 144;
        private const int PLUS_KEY_DEFAULT_WIDTH = 144;
        private const int XL_KEY_DEFAULT_HEIGHT = 96;
        private const int XL_KEY_DEFAULT_WIDTH = 96;
        private const int GENERIC_KEY_IMAGE_SIZE = 144;
        private const string FILENAME_NO_FILE_STRING = "No file...";

        #region Image Related
        /// <summary>
        /// Convert an image file to Base64 format. Set the addHeaderPrefix to true, if this is sent to the SendImageAsync function
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="addHeaderPrefix"></param>
        /// <returns></returns>
        public static string FileToBase64(string fileName, bool addHeaderPrefix)
        {
            if (!File.Exists(fileName)) return null;
            using SKImage image = OpenImageFromFile(fileName);
            return ImageToBase64(image, addHeaderPrefix);
        }

        public static SKImage OpenImageFromFile(string fileName)
        {
            return SKImage.FromEncodedData(fileName);
        }

        /// <summary>
        /// Convert an in-memory image object to Base64 format. Set the addHeaderPrefix to true, if this is sent to the SendImageAsync function
        /// </summary>
        /// <param name="image"></param>
        /// <param name="addHeaderPrefix"></param>
        /// <returns></returns>
        public static string ImageToBase64(SKImage image, bool addHeaderPrefix)
        {
            if (image == null) return null;
            using SKBitmap bitmap = SKBitmap.FromImage(image);
            byte[] imageBytes = bitmap.Bytes;
            string base64String = Convert.ToBase64String(imageBytes);
            return addHeaderPrefix ? HEADER_PREFIX + base64String : base64String;
        }
        
        /// <summary>
        /// Convert an in-memory image object to Base64 format. Set the addHeaderPrefix to true, if this is sent to the SendImageAsync function
        /// </summary>
        /// <param name="image"></param>
        /// <param name="addHeaderPrefix"></param>
        /// <returns></returns>
        public static string ImageToBase64(SKBitmap image, bool addHeaderPrefix)
        {
            if (image == null) return null;
            byte[] imageBytes = image.Bytes;
            string base64String = Convert.ToBase64String(imageBytes);
            return addHeaderPrefix ? HEADER_PREFIX + base64String : base64String;
        }

        /// <summary>
        /// Convert a base64 image string to an Image object
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static SKImage Base64StringToImage(string base64String)
        {
            try
            {
                if (string.IsNullOrEmpty(base64String)) return null;

                // Remove header
                if (base64String[..HEADER_PREFIX.Length] == HEADER_PREFIX)
                {
                    base64String = base64String[HEADER_PREFIX.Length..];
                }

                byte[] imageBytes = Convert.FromBase64String(base64String);
                using var m = new MemoryStream(imageBytes);
                return SKImage.FromEncodedData(m);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.Error, $"Base64StringToImage Exception: {ex}");
            }
            return null;
        }

        /// <summary>
        /// Gets the key default height in pixels.
        /// To get the StreamDeckType use Connection.DeviceInfo()
        /// </summary>
        /// <param name="streamDeckType"></param>
        /// <returns></returns>
        public static int GetKeyDefaultHeight(DeviceType streamDeckType)
        {
            switch (streamDeckType)
            {
                case DeviceType.StreamDeckClassic:
                case DeviceType.StreamDeckMini:
                case DeviceType.StreamDeckMobile:
                case DeviceType.StreamDeckNeo:
                    return CLASSIC_KEY_DEFAULT_HEIGHT;
                case DeviceType.StreamDeckXl:
                    return XL_KEY_DEFAULT_HEIGHT;
                default:
                    Logger.Instance.LogMessage(TracingLevel.Error, $"SDTools GetKeyDefaultHeight Error: Invalid StreamDeckDeviceType: {streamDeckType}");
                    break;
            }
            return 1;
        }

        /// <summary>
        /// Gets the key default width in pixels.
        /// To get the StreamDeckType use Connection.DeviceInfo()
        /// </summary>
        /// <param name="streamDeckType"></param>
        /// <returns></returns>
        public static int GetKeyDefaultWidth(DeviceType streamDeckType)
        {
            switch (streamDeckType)
            {
                case DeviceType.StreamDeckClassic:
                case DeviceType.StreamDeckMini:
                case DeviceType.StreamDeckMobile:
                case DeviceType.StreamDeckNeo:
                    return CLASSIC_KEY_DEFAULT_WIDTH;
                case DeviceType.StreamDeckXl:
                    return XL_KEY_DEFAULT_WIDTH;
                case DeviceType.StreamDeckPlus:
                    return PLUS_KEY_DEFAULT_WIDTH;
                default:
                    Logger.Instance.LogMessage(TracingLevel.Error, $"SDTools GetKeyDefaultHeight Error: Invalid StreamDeckDeviceType: {streamDeckType}");
                    break;
            }
            return 1;
        }

        /// <summary>
        /// Generates an empty key bitmap with the default height and width.
        /// New: To get the StreamDeckType use Connection.DeviceInfo()
        /// </summary>
        /// <param name="streamDeckType"></param>
        /// <returns></returns>
        public static SKData GenerateKeyImage(DeviceType streamDeckType)
        {
            int height = GetKeyDefaultHeight(streamDeckType);
            int width = GetKeyDefaultWidth(streamDeckType);
            return GenerateKeyImage(height, width);
        }

        /// <summary>
        /// Creates a key image that fits all Stream Decks
        /// </summary>
        /// <returns></returns>
        public static SKData GenerateGenericKeyImage()
        {
            return GenerateKeyImage(GENERIC_KEY_IMAGE_SIZE, GENERIC_KEY_IMAGE_SIZE, string.Empty, new TitleParameters(), SKColors.Empty, SKEncodedImageFormat.Png, 80);
        }
        
        /// <summary>
        /// Creates a key image that fits all Stream Decks, with the specified color and font properties.
        /// </summary>
        /// <param name="titleParameters"></param>
        /// <param name="text"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="imageFormat"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        public static SKData GenerateKeyImage(
            TitleParameters titleParameters,
            string text = default,
            SKColor backgroundColor = default,
            SKEncodedImageFormat imageFormat = SKEncodedImageFormat.Png,
            int quality = 80)
        {
            return GenerateKeyImage(GENERIC_KEY_IMAGE_SIZE, GENERIC_KEY_IMAGE_SIZE, text, titleParameters, backgroundColor, imageFormat, quality);
        }
        
        //TODO: CreateAction a version of this that can set a background image from a file path or include that param and logic here.

        /// <summary>
        /// Creates a key image based on given height, width, color and font properties.
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="text"></param>
        /// <param name="titleParameters"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="imageFormat"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        private static SKData GenerateKeyImage(
            int height,
            int width,
            string text = default,
            TitleParameters titleParameters = default,
            SKColor backgroundColor = default,
            SKEncodedImageFormat imageFormat = SKEncodedImageFormat.Png,
            int quality = 80)
        {
            try
            {
                using var bitmap = new SKBitmap(width, height);
                using var canvas = new SKCanvas(bitmap);
                backgroundColor = backgroundColor.Equals(default) ? SKColors.Black : backgroundColor;
                canvas.DrawColor(backgroundColor);
                canvas.AddTextPath(titleParameters, bitmap, text);
                
                return bitmap.Encode(imageFormat, quality);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.Error, $"SDTools GenerateKeyImage exception: {ex} Height: {height} Width: {width}");
            }
            return null;
        }
        #endregion

        #region Filename Related
        /// <summary>
        /// Extracts the actual filename from a file payload received from the Property Inspector
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public static string FilenameFromPayload(JToken payload)
        {
            return FilenameFromString((string)payload);
        }

        private static string FilenameFromString(string filenameWithFakepath)
        {
            if (string.IsNullOrEmpty(filenameWithFakepath)) return null;

            return filenameWithFakepath == FILENAME_NO_FILE_STRING ? 
                string.Empty : 
                Uri.UnescapeDataString(filenameWithFakepath.Replace("C:\\fakepath\\", ""));
        }
        #endregion

        #region String Related
        /// <summary>
        /// Converts a long to a human-readable string. Example: 54,265 => 54.27k
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string FormatNumber(long num)
        {
            return num switch
            {
                >= 100000000 => (num / 1000000D).ToString("0.#M"),
                >= 1000000 => (num / 1000000D).ToString("0.##M"),
                >= 100000 => (num / 1000D).ToString("0.#k"),
                >= 10000 => (num / 1000D).ToString("0.##k"),
                _ => num.ToString("#,0")
            };
        }

        //// <summary>Converts number in bytes to human-readable size (ex. "2 GB")</summary>
        /// <param name="numberInBytes">Size in bytes</param>
        /// <returns>Formatted human-readable string (ex. "2 MB")</returns>
        public static string FormatBytes(double numberInBytes)
        {
            var sizeCounter = 0;
            var format = new[] { "{0:F0} B", "{0:F0} KB", "{0:F0} MB", "{0:F0} GB", "{0:F2} TB", "{0:F2} PB", "{0:F2} EB" };
            while (sizeCounter < format.Length && numberInBytes >= 1024d)
            {
                numberInBytes = 100d * numberInBytes / 1024d / 100d;
                sizeCounter++;
            }
            return string.Format(format[sizeCounter], numberInBytes);
        }
        #endregion

        #region SHA512
        /// <summary>
        /// Returns SHA512 Hash from an image object
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ImageToSha512(SKData data)
        {
            if (data == null) return null;
            
            try
            {
                return BytesToSha512(data.ToArray());
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.Error, $"ImageToSHA512 Exception: {ex}");
            }
            return null;
        }

        /// <summary>
        /// Returns SHA512 Hash from a string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StringToSha512(string str)
        {
            return str == null ? null : BytesToSha512(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// Returns SHA512 Hash from a byte stream
        /// </summary>
        /// <param name="byteStream"></param>
        /// <returns></returns>
        public static string BytesToSha512(byte[] byteStream)
        {
            try
            {
                byte[] hash = SHA512.HashData(byteStream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.Error, $"BytesToSHA512 Exception: {ex}");
            }
            return null;
        }
        #endregion
        
        // TODO: This needs to be refactored to be AOT friendly. Also, the NewtonSoft JSON will be replaced by System.Text.Json
        //   The inner settings classes without this generic type crap is problematic for source generated JSON serialization because
        //   that relies on known classes into which the JSON can be deserialized. The inner classes for settings are always different.
        //   That's probably why these hoops are necessary.
        #region JObject Related
        /// <summary>
        /// Iterates through the fromJObject, finds the property that matches in the toSettings object, and sets the value from the fromJObject object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSettings"></param>
        /// <param name="fromJObject"></param>
        /// <returns>Number of properties updated</returns>
        public static int AutoPopulateSettings<T>(T toSettings, JObject fromJObject)
        {
            Dictionary<string, PropertyInfo> dicProperties = MatchPropertiesWithJsonProperty(toSettings);
            var totalPopulated = 0;

            if (fromJObject == null) return totalPopulated;
            
            foreach (KeyValuePair<string, JToken> prop in fromJObject)
            {
                if (!dicProperties.TryGetValue(prop.Key, out PropertyInfo info)) continue;
                    
                // Special handling for FilenameProperty
                if (info.GetCustomAttributes(typeof(FilenamePropertyAttribute), true).Length > 0)
                {
                    string value = FilenameFromString((string)prop.Value);
                    info.SetValue(toSettings, value);
                }
                else
                {
                    info.SetValue(toSettings, Convert.ChangeType(prop.Value, info.PropertyType));
                }
                totalPopulated++;
            }
            
            return totalPopulated;
        }

        private static Dictionary<string, PropertyInfo> MatchPropertiesWithJsonProperty<T>(T obj)
        {
            var dicProperties = new Dictionary<string, PropertyInfo>();
            if (obj == null) return dicProperties;
            
            PropertyInfo[] props = typeof(T).GetProperties();
            
            foreach (PropertyInfo prop in props)
            {
                object[] attributes = prop.GetCustomAttributes(true);
                foreach (object attr in attributes)
                {
                    if (attr is not JsonPropertyAttribute jprop) continue;
                        
                    dicProperties.Add(jprop.PropertyName ?? string.Empty, prop);
                    break;
                }
            }

            return dicProperties;
        }
        #endregion

        #region Dials Related
        /// <summary>
        /// Takes a custom range and recalculates the value on a scale from 0 to 100
        /// </summary>
        /// <param name="value"></param>
        /// <param name="originalMin"></param>
        /// <param name="originalMax"></param>
        /// <returns></returns>
        public static int RangeToPercentage(int value, int originalMin, int originalMax)
        {
            if (originalMax - originalMin == 0) // Prevent division by zero
            {
                return 0;
            }

            return ((value - originalMin) * 100) / (originalMax - originalMin);
        }
        #endregion

        #region Plugin Helper Classes
        internal static string GetExeName()
        {
            try
            {
                return Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule?.FileName);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.Warn, $"GetExeName failed {ex}");
            }
            return string.Empty;
        }
        #endregion
    }
}
