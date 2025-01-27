using System;
using System.Threading.Tasks;
using BarRaider.SdTools.Communication;
using BarRaider.SdTools.Communication.SDEvents;
using BarRaider.SdTools.Payloads;
using BarRaider.SdTools.Wrappers;
using Newtonsoft.Json.Linq;

namespace BarRaider.SdTools.Utilities
{
    /// <summary>
    /// Helper class which allows fetching the GlobalSettings of a plugin
    /// </summary>
    public class GlobalSettingsManager
    {
        #region Private Static Members
        private static GlobalSettingsManager _instance;
        private static readonly object ObjLock = new();
        #endregion

        #region Private Members
        private const int GET_GLOBAL_SETTINGS_DELAY_MS = 300;
        private StreamDeckConnection streamDeckConnection;
        private readonly System.Timers.Timer tmrGetGlobalSettings = new();
        #endregion

        #region Constructor
        /// <summary>
        /// Returns singleton entry of GlobalSettingsManager
        /// </summary>
        public static GlobalSettingsManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                
                lock (ObjLock)
                {
                    return _instance ??= new GlobalSettingsManager();
                }
            }
        }

        private GlobalSettingsManager()
        {
            tmrGetGlobalSettings.Interval = GET_GLOBAL_SETTINGS_DELAY_MS;
            tmrGetGlobalSettings.Elapsed += TmrGetGlobalSettings_Elapsed;
            tmrGetGlobalSettings.AutoReset = true;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Event triggered when Global Settings are received
        /// </summary>
        public event EventHandler<ReceivedGlobalSettingsPayload> OnReceivedGlobalSettings;
        
        internal void Initialize(StreamDeckConnection connection, int getGlobalSettingsDelayMs = GET_GLOBAL_SETTINGS_DELAY_MS)
        {
            this.streamDeckConnection = connection;
            this.streamDeckConnection.OnDidReceiveGlobalSettings += StreamDeckConnectionOnDidReceiveGlobalSettings;

            tmrGetGlobalSettings.Stop();
            tmrGetGlobalSettings.Interval = getGlobalSettingsDelayMs;
            Logger.Instance.LogMessage(TracingLevel.Info, "GlobalSettingsManager initialized");
        }

        /// <summary>
        /// Command to request the Global Settings. Use the OnDidReceiveGlobalSSettings callback function to receive the Global Settings.
        /// </summary>
        /// <returns></returns>
        public void RequestGlobalSettings()
        {
            if (streamDeckConnection == null)
            {
                Logger.Instance.LogMessage(TracingLevel.Error, "GlobalSettingsManager::RequestGlobalSettings called while streamDeckConnection is null");
                return;
            }

            Logger.Instance.LogMessage(TracingLevel.Info, "GlobalSettingsManager::RequestGlobalSettings called");
            tmrGetGlobalSettings.Start();
        }

        /// <summary>
        /// Sets the Global Settings for the plugin
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="triggerDidReceiveGlobalSettings"></param>
        /// <returns></returns>
        public async Task SetGlobalSettings(JObject settings, bool triggerDidReceiveGlobalSettings = true)
        {
            if (streamDeckConnection == null)
            {
                Logger.Instance.LogMessage(TracingLevel.Error, "GlobalSettingsManager::SetGlobalSettings called while streamDeckConnection is null");
                return;
            }

            Logger.Instance.LogMessage(TracingLevel.Info, "GlobalSettingsManager::SetGlobalSettings called");
            await streamDeckConnection.SetGlobalSettingsAsync(settings);

            if (triggerDidReceiveGlobalSettings)
            {
                tmrGetGlobalSettings.Start();
            }
        }
        #endregion

        #region Private Methods
        private void StreamDeckConnectionOnDidReceiveGlobalSettings(object sender, SdEventReceivedEventArgs<DidReceiveGlobalSettingsEvent> e)
        {
            OnReceivedGlobalSettings?.Invoke(this, JObject.FromObject(e.Event.Payload).ToObject<ReceivedGlobalSettingsPayload>());
        }

        private async void TmrGetGlobalSettings_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            tmrGetGlobalSettings.Stop();

            Logger.Instance.LogMessage(TracingLevel.Info, "GlobalSettingsManager::GetGlobalSettingsAsync triggered");
            await streamDeckConnection.GetGlobalSettingsAsync();
        }
        #endregion
    }
}
