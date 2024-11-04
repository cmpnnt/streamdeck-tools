using System;

namespace BarRaider.SdTools.Wrappers
{
    /// <summary>
    /// Base (Generic) EventArgs used for events
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SdEventReceivedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Event Information
        /// </summary>
        public T Event { get; private set; }
        internal SdEventReceivedEventArgs(T evt)
        {
            Event = evt;
        }
    }
}
