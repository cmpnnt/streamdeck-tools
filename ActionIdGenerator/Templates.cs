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
               namespace Cmpnnt.SdTools.Generators
               {
                   public static class PluginActionIdRegistry
                   {
                       public static List<(Type ClassType, string ActionId)> GetPluginActionIds(){
                           return new List<(Type, string)>
                           {{{string.Join(",\n            ", actionIds.Select(a => $"(Type.GetType(\"{a.ClassName}\"), \"{a.Argument}\")"))}}};
                        }
                   }
               }
               """;
    }
}

