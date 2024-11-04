using System;
using BarRaider.SdTools.Payloads;
using BarRaider.SdTools.Utilities;
using BarRaider.SdTools.Wrappers;
using CommandLine;

namespace BarRaider.SdTools.Backend
{
    /// <summary>
    /// * Easy Configuration Instructions:
    ///* 1. Use NuGet to get the following packages: 
    ///*          CommandLineParser by gsscoder
    ///*          streamdeck-client-csharp by Shane DeSeranno
    ///*          Newtonsoft.Json by James Newton-King
    ///* 2. Create a class that implements the IPluginable interface (which is located in BarRaider.SDTools), this will be your main plugin
    ///* 3. Pass the type of the class to the main function
    /// </summary>
    public static class SdWrapper
    {
        // Handles all the communication with the plugin
        private static PluginContainer _container;

        /// /************************************************************************
        /// * Initial configuration from TyrenDe's streamdeck-client-csharp example:
        /// * https://github.com/TyrenDe/streamdeck-client-csharp
        /// * and SaviorXTanren's MixItUp.StreamDeckPlugin:
        /// * https://github.com/SaviorXTanren/mixer-mixitup/
        /// *************************************************************************/


        /// <summary>
        /// Library's main initialization point. 
        /// Pass the args from your Main function. We'll handle the rest
        /// </summary>
        /// <param name="args"></param>
        /// <param name="updateHandler"></param>
        public static void Run(string[] args, IUpdateHandler updateHandler = null)
        {
            Run(args, Tools.AutoLoadPluginActions(), updateHandler);
        }

        /// <summary>
        /// Library's main initialization point. 
        /// Pass the args from your Main function and a list of supported PluginActionIds, the framework will handle the rest.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="supportedActionIds"></param>
        /// /// <param name="updateHandler"></param>
        private static void Run(string[] args, PluginActionId[] supportedActionIds, IUpdateHandler updateHandler)
        {
            Logger.Instance.LogMessage(TracingLevel.Info, $"Plugin [{Tools.GetExeName()}] Loading - {supportedActionIds.Length} Actions Found");
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            #if DEBUG
            Logger.Instance.LogMessage(TracingLevel.Debug, $"Plugin Loading - Args: {string.Join(" ", args)}");
            #endif

            // The command line args parser expects all args to use `--`, so, let's append
            for (var count = 0; count < args.Length; count++)
            {
                if (args[count].StartsWith("-") && !args[count].StartsWith("--"))
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

            var options = parser.ParseArguments<StreamDeckOptions>(args);
            options.WithParsed(o => RunPlugin(o, supportedActionIds, updateHandler));
        }


        private static void RunPlugin(StreamDeckOptions options, PluginActionId[] supportedActionIds, IUpdateHandler updateHandler)
        {
            _container = new PluginContainer(supportedActionIds, updateHandler);
            _container.Run(options);
        }

        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Instance.LogMessage(TracingLevel.Fatal, $"Unhandled Exception: {e.ExceptionObject}");
        }
    }
}
