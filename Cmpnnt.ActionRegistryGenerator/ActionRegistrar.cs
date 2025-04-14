using System.Linq;
using System.Text;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Threading;

namespace Cmpnnt.ActionRegistryGenerator;

[Generator]
public class PluginRegistrar : IIncrementalGenerator
{
    // Define the fully qualified name of the interface we're looking for
    private const string INTERFACE_FULL_NAME = "BarRaider.SdTools.Backend.ICommonPluginFunctions";
    // REMOVED: ACTION_ID_FIELD_NAME constant is no longer needed.
    // private const string ACTION_ID_FIELD_NAME = "ActionId"; 

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 1. Find all class declarations.
        IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax, // Find any class declaration
                transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node // Select the class node
            );

        // 2. Combine with compilation to get semantic models.
        IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndClasses =
            context.CompilationProvider.Combine(classDeclarations.Collect());

        // 3. Filter classes and extract required data (now just the full class name).
        //    The type parameter is changed to ImmutableArray<string>.
        IncrementalValueProvider<ImmutableArray<string>> pluginClassNames =
            compilationAndClasses.Select((tuple, ct) =>
                GetPluginClassNames(tuple.Item1, tuple.Item2, ct)); // Renamed method for clarity

        // 4. Register the source output.
        //    Pass the new data structure (ImmutableArray<string>) to Generate.
        context.RegisterSourceOutput(pluginClassNames, Generate);
    }

    // Renamed method and changed return type to ImmutableArray<string>
    private static ImmutableArray<string> GetPluginClassNames(
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> classes,
        CancellationToken ct)
    {
        // Builder now holds strings (the fully qualified class names)
        ImmutableArray<string>.Builder results = ImmutableArray.CreateBuilder<string>();

        INamedTypeSymbol? interfaceSymbol = compilation.GetTypeByMetadataName(INTERFACE_FULL_NAME);

        if (interfaceSymbol == null)
        {
            // Interface not found in compilation, cannot proceed.
            // Optionally report a diagnostic here.
            return results.ToImmutable();
        }

        foreach (ClassDeclarationSyntax? classDecl in classes)
        {
            ct.ThrowIfCancellationRequested();

            SemanticModel semanticModel = compilation.GetSemanticModel(classDecl.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(classDecl, ct) is not INamedTypeSymbol classSymbol)
            {
                continue; // Skip if symbol resolution fails
            }

            // Check if the class is concrete (not abstract, static) and implements the target interface.
            // Using OriginalDefinition is robust against constructed generic types if the interface itself were generic.
            if (classSymbol.IsAbstract || classSymbol.IsStatic ||
                !classSymbol.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i.OriginalDefinition, interfaceSymbol.OriginalDefinition)))
            {
                continue; // Skip abstract/static classes or those not implementing the interface
            }

            // REMOVED: Logic to find the ActionId constant field.

            // Get the fully qualified class name.
            string className = classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            // Clean up the "global::" prefix if it exists.
            className = className.Replace("global::", "");

            // Ensure we got a valid class name (should always be true if symbol resolution worked)
            if (!string.IsNullOrEmpty(className))
            {
                // Add the fully qualified class name directly to the results.
                results.Add(className);
            }
        }

        return results.ToImmutable();
    }

    // Updated signature to accept ImmutableArray<string>
    private static void Generate(SourceProductionContext context, ImmutableArray<string> pluginClassNames)
    {
        if (pluginClassNames.IsEmpty)
        {
            return;
        }

        // *** CRITICAL NOTE ***
        // You MUST update the 'Templates.CreateRegistrar' method
        // to accept 'ImmutableArray<string> pluginClassNames' instead of the previous tuple structure.
        // It should now use the strings directly as the Action IDs in the generated code.
        // Example assumption for Templates.CreateRegistrar(pluginClassNames):
        // It might iterate through pluginClassNames and use each string where ActionId was previously used.

        SourceText sourceText = SourceText.From(Templates.CreateRegistrar(pluginClassNames), Encoding.UTF8);
        context.AddSource("PluginActionIdRegistry.g.cs", sourceText);
    }
}