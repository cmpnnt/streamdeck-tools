using System;
using BarRaider.SdTools.Payloads;
using BarRaider.SdTools.Utilities;
using CommandLine;

namespace BarRaider.SdTools.Backend
{
    /// <summary>
    /// * Configuration Instructions: TODO
    /// </summary>
    public static class SdWrapper
    {
        // Handles all the communication with the plugin
        private static PluginContainer _container;

        /// /************************************************************************
        /// Based on Barraider's C# StreamDeck tools, which in turn was based on:
        ///   * Initial configuration from TyrenDe's streamdeck-client-csharp example:
        ///   * https://github.com/TyrenDe/streamdeck-client-csharp
        ///   *  and SaviorXTanren's MixItUp.StreamDeckPlugin:
        ///   * https://github.com/SaviorXTanren/mixer-mixitup/
        /// *************************************************************************/

        /// <summary>
        /// Library's main initialization point. 
        /// Pass the args from your Main function and a list of supported PluginActionIds, the framework will handle the rest.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="registry"></param>
        /// /// <param name="updateHandler"></param>
        public static void Run(string[] args, IPluginActionRegistry registry, IUpdateHandler updateHandler = null)
        {
            Logger.Instance.LogMessage(TracingLevel.Info, $"Plugin [{Tools.GetExeName()}] Loading - {registry.PluginActionIDs().Count} Actions Found");
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            #if DEBUG
            Logger.Instance.LogMessage(TracingLevel.Debug, $"Plugin Loading - Args: {string.Join(" ", args)}");
            #endif

            // The command line args parser expects all args to use `--`, so, let's append
            for (var count = 0; count < args.Length; count++)
            {
                if (args[count].StartsWith('-') && !args[count].StartsWith("--"))
                {
                    args[count] = $"-{args[count]}";
                }
            }

            var parser = new Parser((with) =>
            {
                with.EnableDashDash = true;
                with.CaseInsensitiveEnumValues = true;
                with.CaseSensitive = false;
                with.IgnoreUnknownArguments = true;
                with.HelpWriter = Console.Error;
            });

            ParserResult<StreamDeckOptions> options = parser.ParseArguments<StreamDeckOptions>(args);
            options.WithParsed(o => RunPlugin(o, registry, updateHandler));
        }

        private static void RunPlugin(StreamDeckOptions options, IPluginActionRegistry registry, IUpdateHandler updateHandler)
        {
            _container = new PluginContainer(registry, updateHandler);
            _container.Run(options);
        }

        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Instance.LogMessage(TracingLevel.Fatal, $"Unhandled Exception: {e.ExceptionObject}");
        }
    }
}
