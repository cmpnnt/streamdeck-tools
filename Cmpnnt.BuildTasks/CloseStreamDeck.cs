using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Cmpnnt.BuildTasks.Utilities;
using Microsoft.Build.Framework;
using Task = Microsoft.Build.Utilities.Task;

namespace Cmpnnt.BuildTasks;

public class CloseStreamDeck : Task
{
    /// <summary>
    /// The name of your plugin, used to stop the process.
    /// </summary>
    [Required]
    public string PluginName { get; set; }

    [Output] 
    public bool ClosedStreamDeck { get; set; } = false;
    
    public override bool Execute()
    {
        // This is a `BeforeBuild` task to stop the streamdeck and the plugin instance, because
        // the output can't be copied if it's running. It can poll a few times to ensure the SD
        // app and plugin isn't running, or throw an error if it doesn't stop. The task should
        // have an output property denoting whether the SD app and plugin were closed successfully.
        // First, it should check should that the SD CLI exists, so we don't bother killing the
        // SD application if we can't link the plugin. If neither the SD app nor plugin were running,
        // set `ClosedStreamDeck` to true. If either one was running and was successfully stopped,
        // set it to true. This will also run only in debug.
        
        ProcessUtilities pu = new(PluginName, this);
        
        if (!pu.FindCli()) return false;
        if (!pu.StopPlugin()) return false;
        if (!pu.StopStreamDeck()) return false;

        ClosedStreamDeck = true; // I might not need this output if I can use the return value of this method
        return true;
    }
}