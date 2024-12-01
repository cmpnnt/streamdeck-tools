using System.Collections.Immutable;
using System.Linq;

namespace ActionIdGenerator;

internal static class Templates
{
    public static string GetPluginActionIds(ImmutableArray<(string ClassName, string Argument)> actionIds)
    {
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

