using System;
using Cmpnnt.BuildTasks.Utilities;
using Task = Microsoft.Build.Utilities.Task;

namespace Cmpnnt.BuildTasks;

public class OpenStreamDeck : Task
{
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
        
        string appPath = Environment.OSVersion.Platform switch
        {
            PlatformID.Win32NT => @"C:\Program Files\Elgato\StreamDeck\StreamDeck.exe",
            PlatformID.MacOSX => "",
            _ => throw new NotSupportedException() // Linux: Wishful thinking
        };
        
        ProcessUtilities pu = new("", this);
        return pu.StartStreamDeck(appPath);
    }
}