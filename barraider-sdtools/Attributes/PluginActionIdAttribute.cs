using System;

namespace BarRaider.SdTools.Attributes
{
    /// <summary>
    /// PluginActionId attribute
    /// Used to indicate the UUID in the manifest file that matches to this class
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PluginActionIdAttribute : Attribute
    {

        /// <summary>
        /// UUID of the action
        /// </summary>
        public string ActionId { get; private set; }

        /// <summary>
        /// Constructor - This attribute is used to indicate the UUID in the manifest file that matches to this class
        /// </summary>
        /// <param name="actionId"></param>
        public PluginActionIdAttribute(string actionId)
        {
            ActionId = actionId;
        }
    }
}
