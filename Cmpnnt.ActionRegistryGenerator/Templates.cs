﻿using System.Collections.Immutable;
using System.Text;

namespace Cmpnnt.ActionRegistryGenerator;

internal static class Templates
    {
        public static string CreateRegistrar(ImmutableArray<string> plugins)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"// <auto-generated/>
using System;
using BarRaider.SdTools;
using BarRaider.SdTools.Backend;
using BarRaider.SdTools.Payloads;
using BarRaider.SdTools.Wrappers;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace Cmpnnt.SdTools.Generators
{
    public class PluginActionIdRegistry : IPluginActionRegistry
    {
        private static readonly FrozenDictionary<string, Func<ISdConnection, InitialPayload, ICommonPluginFunctions>> _actionFactories = CreateFactories();
        private static readonly FrozenSet<string> _actionIds = CreateActionIdSet();

        public FrozenSet<string> PluginActionIDs() => _actionIds;

        public ICommonPluginFunctions CreateAction(string actionId, ISdConnection connection, InitialPayload payload)
        {
             if (_actionFactories.TryGetValue(actionId, out var factory))
             {
                 return factory(connection, payload);
             }
             // Consider logging or throwing an exception for unknown actionId
             return null;
        }

        private static FrozenSet<string> CreateActionIdSet()
        {
             var actions = new HashSet<string>()
             {");

            // Add each Action ID to the HashSet initializer
            foreach (string actionId in plugins)
            {
                sb.AppendLine($"                \"{EscapeString(actionId.ToLower())}\",");
            }

            sb.AppendLine(@"            };
             return actions.ToFrozenSet();
        }

        private static FrozenDictionary<string, Func<ISdConnection, InitialPayload, ICommonPluginFunctions>> CreateFactories()
        {
            var factories = new Dictionary<string, Func<ISdConnection, InitialPayload, ICommonPluginFunctions>>()
            {");

            // Add each factory function to the Dictionary initializer
            foreach (string actionId in plugins)
            {
                // Note: Using fully qualified class name with global:: prefix
                sb.AppendLine($"                [\"{EscapeString(actionId.ToLower())}\"] = (conn, load) => new {actionId}(conn, load),");
            }

            sb.AppendLine(@"            };
            return factories.ToFrozenDictionary();
        }
    }
}");
            return sb.ToString();
        }

        // Helper to escape strings for C# source code
        private static string EscapeString(string value)
        {
            return value.Replace("\"", "\\\"");
        }
    }