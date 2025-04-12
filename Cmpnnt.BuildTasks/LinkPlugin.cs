using System;
using System.Runtime.InteropServices;
using Cmpnnt.BuildTasks.Utilities;
using Microsoft.Build.Framework;
using Task = Microsoft.Build.Utilities.Task;

namespace Cmpnnt.BuildTasks;

public class LinkPlugin : Task
{
    /// <summary>
    /// The name of your plugin, used to stop the process.
    /// </summary>
    [Required]
    public string PluginName { get; set; }
    
    /// <summary>
    /// The path to your plugin's build output directory.
    /// </summary>
    [Required]
    public string BuildDir { get; set; }
    
    public override bool Execute()
    {
        // TODO: This is an `AfterBuild` task to replace the install script. First, it
        // checks if the CLI app exists, and skip the task if not. It then deletes the
        // existing link in the SD plugins directory, if present, and recreates it with
        // the build output path (using the SD cli). It should have default plugin paths
        // for both Mac and Windows, and a property allowing that path to be specified.
        // Finally, it will start the streamdeck application. This task should take as an
        // input whether or not the BeforeBuild task was able to kill the SD app and plugin.
        // Skip the SD restart if not. This should only run in debug configuration.
        
        ProcessUtilities pu = new(PluginName, this);

        if (pu.IsRunning("StreamDeck"))
        {
            Log.LogError("StreamDeck is running. Please close the application before continuing.");
            return false;
        }
        
        return pu.FindCli() && pu.LinkPlugin(BuildDir);
    }
}