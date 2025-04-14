using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Task = Microsoft.Build.Utilities.Task;
using Microsoft.Build.Framework;

namespace Cmpnnt.BuildTasks;

public class GenerateManifest : Task
{
    // Input property from the MSBuild project file:
    // Path to the compiled assembly of the main plugin project
    [Required]
    public string PluginAssemblyPath { get; set; }

    // Input property from the MSBuild project file:
    // Desired output path for manifest.json
    [Required]
    public string OutputManifestPath { get; set; }

    // Optional input: Namespace of the generated provider
    public string GeneratedProviderNamespace { get; set; } = "GeneratedManifest";

    // Optional input: Class name of the generated provider
    public string GeneratedProviderClass { get; set; } = "ManifestProvider";

    // Optional input: Method name to get the data object
    public string GeneratedProviderMethod { get; set; } = "GetManifestData";

    public override bool Execute()
    {
        if (string.IsNullOrEmpty(PluginAssemblyPath) || string.IsNullOrEmpty(OutputManifestPath))
        {
            Log.LogError("PluginAssemblyPath and OutputManifestPath properties are required.");
            return false;
        }

        if (!File.Exists(PluginAssemblyPath))
        {
            Log.LogError($"Plugin assembly not found at '{PluginAssemblyPath}'. Skipping manifest generation. This might happen on initial builds before the assembly exists.");
            return true;
        }

        try
        {
            Log.LogMessage(MessageImportance.Normal, $"Loading assembly: {PluginAssemblyPath}");
            // Load the assembly into a separate context to avoid locking files if possible,
            // although for build tasks, direct loading is often fine.
            Assembly assembly = Assembly.LoadFrom(PluginAssemblyPath); // Consider using MetadataLoadContext if only reading metadata

            var providerFullName = $"{GeneratedProviderNamespace}.{GeneratedProviderClass}";
            Log.LogMessage(MessageImportance.Normal, $"Looking for type: {providerFullName}");
            Type providerType = assembly.GetType(providerFullName);

            if (providerType == null)
            {
                Log.LogError($"Could not find generated type '{providerFullName}' in assembly '{PluginAssemblyPath}'. Ensure the source generator ran successfully.");
                return false;
            }

            Log.LogMessage(MessageImportance.Normal, $"Looking for method: {GeneratedProviderMethod}");
            MethodInfo getDataMethod = providerType.GetMethod(GeneratedProviderMethod, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic); // Adjust flags if needed

            if (getDataMethod == null)
            {
                Log.LogError($"Could not find static method '{GeneratedProviderMethod}' on type '{providerFullName}'.");
                return false;
            }

            Log.LogMessage(MessageImportance.Normal, "Invoking GetManifestData method...");
            object manifestData = getDataMethod.Invoke(null, null);

            if (manifestData == null)
            {
                Log.LogError($"'{GeneratedProviderMethod}' returned null.");
                return false;
            }

            Log.LogMessage(MessageImportance.Normal, $"Serializing manifest data to JSON...");
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                // Ensure encoder allows characters often found in paths/names if necessary
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                // Include nulls or not Usually better to omit for manifest.json
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            string jsonContent = JsonSerializer.Serialize(manifestData, options);

            // Ensure directory exists
            string outputDir = Path.GetDirectoryName(OutputManifestPath);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                 Log.LogMessage(MessageImportance.Normal, $"Creating directory: {outputDir}");
                 Directory.CreateDirectory(outputDir);
            }

            Log.LogMessage(MessageImportance.High, $"Writing manifest file: {OutputManifestPath}");
            File.WriteAllText(OutputManifestPath, jsonContent);

            return true; // Success
        }
        catch (Exception ex)
        {
            Log.LogErrorFromException(ex, true, true, null);
            return false; // Failure
        }
    }
}