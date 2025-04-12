using System;
using System.Runtime.InteropServices;
using Cmpnnt.BuildTasks.Utilities;
using Microsoft.Build.Framework;
using Task = Microsoft.Build.Utilities.Task;

namespace Cmpnnt.BuildTasks;

public class PackagePlugin : Task
{
    /// <summary>
    /// The path to your plugin's build directory.
    /// </summary>
    [Required]
    public string BuildDir { get; set; }
    
    /// <summary>
    /// The destination for your packaged plugin.
    /// </summary>
    public string OutputDir { get; set; } = PluginsDirectory();
    
    public override bool Execute()
    {
        ProcessUtilities pu = new("", this);

        if (pu.IsRunning("StreamDeck"))
        {
            Log.LogError("StreamDeck is running. Please close the application before continuing.");
            return false;
        }
        
        return pu.FindCli() && pu.PackPlugin(BuildDir, OutputDir);
    }
    
    // TODO: Note the default directories for the packaged plugin in the documentation.
    private static string PluginsDirectory()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return @$"{appData}\Elgato\StreamDeck\Plugins";
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "~/Library/Application Support/Elgato/StreamDeck/Plugins";
        }

        throw new PlatformNotSupportedException();
    }
}