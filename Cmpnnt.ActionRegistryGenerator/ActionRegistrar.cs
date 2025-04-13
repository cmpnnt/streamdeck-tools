using System.Linq;
using System.Text;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Threading; // Required for CancellationToken

namespace Cmpnnt.ActionRegistryGenerator;

[Generator]
public class PluginRegistrar : IIncrementalGenerator
{
    // Define the fully qualified name of the interface we're looking for
    private const string INTERFACE_FULL_NAME = "BarRaider.SdTools.Backend.ICommonPluginFunctions";
    // Define the name of the constant field we expect for the Action ID
    private const string ACTION_ID_FIELD_NAME = "ActionId";

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

        // 3. Filter classes and extract required data.
        IncrementalValueProvider<ImmutableArray<(string ClassName, string ActionId)>> pluginData =
            compilationAndClasses.Select((tuple, ct) =>
                GetPluginClasses(tuple.Item1, tuple.Item2, ct));

        // 4. Register the source output.
        context.RegisterSourceOutput(pluginData, Generate);
    }

    private static ImmutableArray<(string ClassName, string ActionId)> GetPluginClasses(
        Compilation compilation, 
        ImmutableArray<ClassDeclarationSyntax> classes, 
        CancellationToken ct)
    {
        ImmutableArray<(string ClassName, string ActionId)>.Builder results = 
            ImmutableArray.CreateBuilder<(string ClassName, string ActionId)>();
        
        INamedTypeSymbol? interfaceSymbol = compilation.GetTypeByMetadataName(INTERFACE_FULL_NAME);

        if (interfaceSymbol == null)
        {
            // Interface not found in compilation, cannot proceed.
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
            if (classSymbol.IsAbstract || classSymbol.IsStatic ||
                !classSymbol.AllInterfaces.Contains(interfaceSymbol, SymbolEqualityComparer.Default))
            {
                continue; // Skip abstract/static classes or those not implementing the interface
            }

            // Find the required constant field (ActionId)
            ISymbol? actionIdMember = classSymbol.GetMembers(ACTION_ID_FIELD_NAME)
                .FirstOrDefault(m => m is IFieldSymbol { IsConst: true } fs && // Must be const
                                     fs.DeclaredAccessibility == Accessibility.Public && // Must be public
                                     fs.Type.SpecialType == SpecialType.System_String); // Must be a string

            if (actionIdMember is not IFieldSymbol actionIdField || actionIdField.ConstantValue == null)
            {
                // Optionally report a diagnostic: Class implements interface but lacks the required public const string ActionId field.
                continue; // Skip if ActionId constant is missing, not public, not const, not a string, or has no value
            }

            var actionIdValue = actionIdField.ConstantValue.ToString();
            string className = classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat); // Get fully qualified name

            // Ensure we got valid data
            if (!string.IsNullOrEmpty(actionIdValue) && !string.IsNullOrEmpty(className))
            {
                 results.Add((ClassName: $"{className}", ActionId: actionIdValue));
            }
        }

        return results.ToImmutable();
    }

    // Note: The signature of Generate now matches the data structure from GetPluginClasses
    private static void Generate(SourceProductionContext context, ImmutableArray<(string ClassName, string ActionId)> plugins)
    {
        if (plugins.IsEmpty)
        {
            return;
        }

        // Assume 'Templates.CreateRegistrar' is updated to accept this new structure
        // You will need to modify the Templates class accordingly.
        SourceText sourceText = SourceText.From(Templates.CreateRegistrar(plugins), Encoding.UTF8);
        context.AddSource("PluginActionIdRegistry.g.cs", sourceText); // Consistent naming with example output
    }
}
