﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ActionIdGenerator;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

[Generator]
public class PluginActionIdGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Create a pipeline to collect class declarations with PluginActionIdAttribute
        IncrementalValuesProvider<(SemanticModel SemanticModel, ClassDeclarationSyntax?)> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                transform: static (context, _) => context)
            .Where(ctx => ctx.Node is ClassDeclarationSyntax)
            .Select((ctx, _) => (ctx.SemanticModel, ctx.Node as ClassDeclarationSyntax));

        // Aggregate PluginActionIdAttribute arguments and class names
        IncrementalValueProvider<ImmutableArray<(string className, string argument)>> attributeData = classDeclarations
            .SelectMany((ctx, _) =>
            {
                ClassDeclarationSyntax? classDecl = ctx.Item2;
                SemanticModel? semanticModel = ctx.Item1;
                var className = semanticModel.GetDeclaredSymbol(classDecl)?.ToString();

                return from attributeList in classDecl.AttributeLists
                    from attribute in attributeList.Attributes
                    where attribute.Name.ToString().Contains("PluginActionId")
                    let argument = attribute.ArgumentList?.Arguments.FirstOrDefault()?.ToString().Trim('"')
                    where argument != null
                    select (className, argument);
            })
            .Collect();

        // Register the source output
        context.RegisterSourceOutput(attributeData, Generate); // Generate is a method group. It's the same as `(ctx, actionIds) => Generate(ctx, actionIds)`
    }

    private void Generate(SourceProductionContext context, ImmutableArray<(string ClassName, string Argument)> actionIds)
    {
        // Generate a new class with a method that returns the extracted arguments
        SourceText actionIdSource = SourceText.From(Templates.GetPluginActionIds(actionIds), Encoding.UTF8);
        context.AddSource($"PluginActionIdRegistry.g.cs", actionIdSource);
    }
}

