using System;
using System.IO;
using Microsoft.Build.Framework;
using Task = Microsoft.Build.Utilities.Task;

namespace Cmpnnt.BuildTasks;

public class RenameOutputDirectory : Task
{
    [Required]
    public string OldPath { get; set; }
        
    [Required]
    public string NewPath { get; set; }

    public override bool Execute()
    {
        if(Directory.Exists(NewPath))
        {
            Directory.Delete(NewPath, true);
        }

        try
        {
            Directory.Move(OldPath, NewPath);
        }
        catch (Exception e)
        {
            Log.LogErrorFromException(e);
        }
            
        return true;
    }
}