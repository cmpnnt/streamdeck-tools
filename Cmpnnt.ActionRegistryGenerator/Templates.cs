using System.Collections.Immutable;
using System.Linq;

namespace Cmpnnt.ActionRegistryGenerator;

internal static class Templates
{
    public static string CreateRegistrar(ImmutableArray<(string ClassName, string Argument)> actions)
    {
        return $$"""
               using System;
               using BarRaider.SdTools.Backend;
               using BarRaider.SdTools.Payloads;
               using BarRaider.SdTools.Wrappers;
               using System.Collections.Frozen;
               using System.Collections.Generic;
               
               namespace Cmpnnt.SdTools.Generators
               {
                   public class PluginActionIdRegistry : IPluginActionRegistry
                   {
                       public FrozenSet<string> PluginActionIDs()
                       {
                           var actions = new HashSet<string>()
                           {{{string.Join(",\n            ", actions.Select(a => $"\"{a.Argument}\""))}}};
                           return actions.ToFrozenSet();
                        }
                        
                        public ICommonPluginFunctions CreateAction(string actionId, ISdConnection connection, InitialPayload payload) => actionId switch
                        {
                           {{string.Join(",\n\t\t\t", actions.Select(action => $"\"{action.Argument}\" => new {action.ClassName}(connection, payload)"))}},
                          _ => null
                        };
                   }
               }
               """;
    }
}
