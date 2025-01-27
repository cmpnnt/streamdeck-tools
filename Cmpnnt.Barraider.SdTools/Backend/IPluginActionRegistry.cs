using System.Collections.Frozen;
using BarRaider.SdTools.Payloads;

namespace BarRaider.SdTools.Backend;


/// <summary>
/// This class is intended to be implemented by a class generated in Cmpnnt.ActionRegistryGenerator
/// </summary>
public interface IPluginActionRegistry
{
    /// <summary>
    /// An immutable set with the PluginAction UUIDs are found in both the plugin's manifest
    /// and in the attribute that decorates each action of your plugin. 
    /// </summary>
    public FrozenSet<string> PluginActionIDs();
    

    /// <summary>
    /// Contains a source-generated factory for each action supported by this plugin.
    /// </summary>
    /// <param name="actionId">The UUID of the PluginAction of which to create a new instance</param>
    /// <param name="connection">Contains the information necessary to connect to the Stream Deck</param>
    /// <param name="payload">The initial payload received during the plugin's construction. Contains basic
    /// information about the plugin.</param>
    /// <returns>An instance of the PluginAction that corresponds to the given <paramref name="actionId"/></returns>
    public ICommonPluginFunctions CreateAction(string actionId, ISdConnection connection, InitialPayload payload);
}