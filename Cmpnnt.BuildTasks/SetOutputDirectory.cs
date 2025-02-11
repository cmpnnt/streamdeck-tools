using System.IO;
using System.Text.Json;
using Microsoft.Build.Framework;
using Task = Microsoft.Build.Utilities.Task;

namespace Cmpnnt.BuildTasks;

public class SetOutputDirectory : Task
{
    [Required]
    public string JsonFilePath { get; set; }

    [Output]
    public string OutputDirectory { get; set; }

    public override bool Execute()
    {
        if (!File.Exists(JsonFilePath))
        {
            Log.LogError($"The file '{JsonFilePath}' does not exist.");
            return false;
        }
            
        JsonDocument jsonObject = JsonDocument.Parse(File.ReadAllText(JsonFilePath));
            
        if (!jsonObject.RootElement.TryGetProperty("UUID", out JsonElement uuidElement))
        {
            Log.LogError("UUID key is missing in manifest.json");
            return false;
        }

        OutputDirectory = uuidElement.GetString(); 
        
        return true;
    }
}