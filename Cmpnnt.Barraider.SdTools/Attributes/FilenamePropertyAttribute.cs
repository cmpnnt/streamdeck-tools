using System;

namespace BarRaider.SdTools.Attributes
{

    /// <summary>
    /// FilenamePropertyAttribute - Used to indicate the current property holds a file name. 
    /// This will allow StreamDeck Utilities to strip the mandatory "C:\fakepath\" added by the SDK
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class FilenamePropertyAttribute : Attribute
    {
    }
}
