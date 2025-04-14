using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Build.Utilities;
using Task = Microsoft.Build.Utilities.Task;

namespace Cmpnnt.BuildTasks.Utilities;

public class ProcessUtilities(string pluginName, Task task)
{
    private readonly TaskLoggingHelper logger = new(task);

    public bool FindCli()
    {
        (bool success, string output) result = CommandLineWrapper.Execute("streamdeck", "-v", true).Result;

        if (!result.success)
        {
            logger.LogError("Cannot find streamdeck CLI installed.");
            return false;
        }
       
        Regex regex = new(@"^\d.\d.\d");
        MatchCollection matches = regex.Matches(result.output);
        
        if (matches.Count == 0)
        {
            logger.LogError("Cannot find streamdeck CLI installed.");
            return false;
        }
        
        logger.LogMessage($"Found Streamdeck CLI version {matches[0].Groups[0].Value}");
        return true;
    }

    /// <summary>
    /// This step is usually unnecessary because the plugin stops with the Stream Deck application,
    /// but we call this to ensure the plugin stops.
    /// </summary>
    /// <returns>True, if plugin process was stopped.</returns>
    public bool StopPlugin()
    {
        Process[] procs = Process.GetProcessesByName(pluginName);
        foreach (Process p in procs)
        {
            try
            {
                p.Kill();
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to stop plugin: {e.Message}");
                return false;
            }
            
            p.WaitForExit();
        }
        return true;
    }

    /// <summary>
    /// Kills running Stream Deck Process
    /// </summary>
    /// <returns>True, if Stream Deck process was stopped</returns>
    public bool StopStreamDeck()
    {
        Process[] procs = Process.GetProcessesByName("StreamDeck");
        foreach (Process p in procs)
        {
            try
            {
                p.Kill();
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to stop Stream Deck: {e.Message}");
                return false;
            }
            p.WaitForExit();
        }
        return true;
    }

    /// <summary>
    /// Installs your plugin with the Stream Deck CLI by linking your build directory
    /// to the Stream Deck plugins directory.
    /// </summary>
    /// <param name="buildDir">The directory containing the compiled plugin to link to the Stream Deck</param>
    /// <returns>True, if successful</returns>
    public bool LinkPlugin(string buildDir)
    {
        (bool success, string output) result = CommandLineWrapper.Execute("streamdeck", $"link {buildDir}", true).Result;
        if (!result.success)
        {
            logger.LogError("Failed linking plugin. The link might already exist.");
        }
        
        return true;
    }
    
    public bool PackPlugin(string buildDir, string outputDir)
    {
        var packCommand = $"pack {buildDir} -o {outputDir}";
        (bool success, string output) result = CommandLineWrapper.Execute("streamdeck", packCommand, true).Result;
        if (!result.success)
        {
            logger.LogError($"Failed to pack plugin. {result.output}");
        }
        
        return true;
    }
    
    /// <summary>
    /// Starts the Stream Deck application.
    /// </summary>
    /// <returns>True, if successful</returns>
    public bool StartStreamDeck(string appPath)
    {
        Process process = Process.Start(appPath);
        return process is { Responding: true };
    }
    
    public bool IsRunning(string processName)
    {
        Process[] procs = Process.GetProcessesByName(processName);
        return procs.Length > 0;
    }
}