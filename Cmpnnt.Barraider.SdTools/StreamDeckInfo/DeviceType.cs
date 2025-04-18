﻿namespace BarRaider.SdTools.StreamDeckInfo
{
    /// <summary>
    /// Type of StreamDeck hardware device, currently two are supported (classic and mini)
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// StreamDeck classic with 15 keys
        /// </summary>
        StreamDeckClassic = 0,

        /// <summary>
        /// StreamDeck mini with 6 keys
        /// </summary>
        StreamDeckMini = 1,

        /// <summary>
        /// StreamDeck XL with 32 keys
        /// </summary>
        StreamDeckXl = 2,

        /// <summary>
        /// StreamDeck Mobile version
        /// </summary>
        StreamDeckMobile = 3,

        /// <summary>
        /// Corsair G-Keys version
        /// </summary>
        CorsairGKeys = 4,

        /// <summary>
        /// Pedal
        /// </summary>
        StreamDeckPedal = 5,

        /// <summary>
        /// Corsair CUE SDK (?)
        /// </summary>
        CorsairCueSdk = 6,

        /// <summary>
        /// Stream Deck+
        /// </summary>
        StreamDeckPlus = 7,
        
        /// <summary>
        /// SCUF Controller.
        /// </summary>
        SCUFController = 8,

        /// <summary>
        /// Stream Deck Neo.
        /// </summary>
        StreamDeckNeo = 9
    }
}
