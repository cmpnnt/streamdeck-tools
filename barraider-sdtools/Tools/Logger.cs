using NLog;

namespace BarRaider.SdTools.Tools
{
    /// <summary>
    /// Tracing levels used for Logger
    /// </summary>
    public enum TracingLevel
    {
        /// <summary>
        /// Debug level
        /// </summary>
        Debug,

        /// <summary>
        /// Informational level
        /// </summary>
        Info,

        /// <summary>
        /// Warning level
        /// </summary>
        Warn,

        /// <summary>
        /// Error level
        /// </summary>
        Error,

        /// <summary>
        /// Fatal (highest) level
        /// </summary>
        Fatal
    }

    /// <summary>
    /// Log4Net logger helper class
    /// </summary>
    public class Logger
    {
        private static Logger _instance = null;
        private static readonly object ObjLock = new object();

        /// <summary>
        /// Returns singelton entry of Log4Net logger
        /// </summary>
        public static Logger Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                lock (ObjLock)
                {
                    if (_instance == null)
                    {
                        _instance = new Logger();
                    }
                    return _instance;
                }
            }
        }

        private readonly NLog.Logger log = null;
        private Logger()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "pluginlog.log", ArchiveEvery=NLog.Targets.FileArchivePeriod.Day, MaxArchiveFiles=3, ArchiveFileName="archive/log.{###}.log", ArchiveNumbering=NLog.Targets.ArchiveNumberingMode.Rolling, Layout = "${longdate}|${level:uppercase=true}|${processname}|${threadid}|${message}" };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            LogManager.Configuration = config;
            log = LogManager.GetCurrentClassLogger();
            LogMessage(TracingLevel.Debug, "Logger Initialized");
        }

        /// <summary>
        /// Add message to log with a specific severity level. 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void LogMessage(TracingLevel level, string message)
        {
            switch (level)
            {
                case TracingLevel.Debug:
                    log.Debug(message);
                    break;

                case TracingLevel.Info:
                    log.Info(message);
                    break;

                case TracingLevel.Warn:
                    log.Warn(message);
                    break;

                case TracingLevel.Error:
                    log.Error(message);
                    break;

                case TracingLevel.Fatal:
                    log.Fatal(message);
                    break;
            }
        }
    }
}
