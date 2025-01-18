using System.Collections.Immutable;
using System.Linq;

namespace ActionIdGenerator;

internal static class Templates
{
    public static string GetPluginActionIds(ImmutableArray<(string ClassName, string Argument)> actionIds)
    {
        // TODO: Add a Factory property to the PluginActionId class and pass it in here
        //   The factory approach is tricky because it still requires a generic, which uses a type
        return $$"""
               using System;
               using System.Collections.Generic;
               using BarRaider.SdTools.Wrappers;
               
               namespace Cmpnnt.SdTools.Generators
               {
                   public static class PluginActionIdRegistry
                   {
                       public static PluginActionId[] AutoLoadPluginActions(){
                           var actions = new List<PluginActionId>
                           {{{string.Join(",\n            ", actionIds.Select(a => $"new PluginActionId(\"{a.Argument}\", Type.GetType(\"{a.ClassName}\"))"))}}};
                           return actions.ToArray();
                        }
                   }
               }
               """;
    }
}

