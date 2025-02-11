using System;
using Microsoft.Build.Framework;
using Task = Microsoft.Build.Utilities.Task;

namespace Cmpnnt.BuildTasks;

public class PackagePlugin : Task
{
    [Required]
    public string PublishedPath { get; set; }
    
    // If this isn't set, default to the build output directory
    public string PluginDestinationPath { get; set; }
    
    public override bool Execute()
    {
        // TODO: This should run the streamdeck command to package the specified directory
        // into a streamdeck plugin. It should check if the `streamdeck` command is available
        // and log the exception, if not installed. This will be an `AfterPublish` task.
        
        throw new NotImplementedException();
    }
}