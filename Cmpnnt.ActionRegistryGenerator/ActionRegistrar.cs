using System.Linq;
using System.Text;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Cmpnnt.ActionRegistryGenerator;

[Generator]
public class PluginRegistrar : IIncrementalGenerator
{
    //
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

                // TODO: Replace this attribute nonsense. Instead, look for a base class that it inherits from.
                return from attributeList in classDecl.AttributeLists
                    from attribute in attributeList.Attributes
                    where attribute.Name.ToString().Contains("PluginActionId") &&
                          semanticModel.GetTypeInfo(attribute).Type?.ToDisplayString() == "BarRaider.SdTools.Attributes.PluginActionIdAttribute"
                    let argument = attribute.ArgumentList?.Arguments.FirstOrDefault()?.ToString().Trim('"')
                    where argument != null
                    select (className, argument);
            })
            .Collect();

        // Register the source output
        context.RegisterSourceOutput(attributeData, Generate); // Generate is a method group. It's the same as `(ctx, actionIds) => Generate(ctx, actionIds)`
    }
    
    private static void Generate(SourceProductionContext context, ImmutableArray<(string, string)> actions)
    {
        if (actions.IsEmpty) return;
        // Generate a new class with a method that returns the extracted arguments
        SourceText actionIdSource = SourceText.From(Templates.CreateRegistrar(actions), Encoding.UTF8);
        context.AddSource($"ActionRegistrar.g.cs", actionIdSource);
    }
}

