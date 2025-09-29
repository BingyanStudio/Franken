using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace Franken.SourceGenerator;

[Generator]
public class UIWindowSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax,
                transform: (generatorSyntaxContext, _) =>
                {
                    var classDeclaration = (ClassDeclarationSyntax)generatorSyntaxContext.Node;
                    var classSymbol = generatorSyntaxContext.SemanticModel.GetDeclaredSymbol(classDeclaration);
                    return new { ClassDeclaration = classDeclaration, ClassSymbol = classSymbol };
                })
            .Where(t => t.ClassDeclaration is not null
                     && t.ClassSymbol.InheritsFrom("UIWindowBase")
                     && t.ClassDeclaration.IsPartialClass())
            .Collect();

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations);

        context.RegisterSourceOutput(compilationAndClasses, (sourceProductionContext, tuple) =>
        {
            foreach (var item in tuple.Right)
            {
                var classDeclaration = item.ClassDeclaration;
                var classSymbol = item.ClassSymbol;

                var source = GenerateSourceCode(classDeclaration, classSymbol);
                sourceProductionContext.AddSource($"{classSymbol.Name}.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        });
    }

    private static string GenerateSourceCode(ClassDeclarationSyntax classDeclaration, INamedTypeSymbol classSymbol)
    {
        StringBuilder sb = new("using Godot;");

        string className = classDeclaration.Identifier.Text, namesp = classSymbol.ContainingNamespace?.ToString() ?? string.Empty;
        var starting = namesp == string.Empty ? string.Empty : $"namespace {namesp};\n";

        sb.Append($@"
using System.Linq;
using System.Collections.Generic;

{starting}
public partial class {className}
{{
    protected override void GetRef()
    {{
        base.GetRef();
        var children = this.GetAllChildren().ToLookup(child => child.Name);");
        foreach (var field in classDeclaration.ChildNodes()
                                              .OfType<FieldDeclarationSyntax>()
                                              .Where(n => n.HasAttribute("UIRef")))
        {
            var fieldType = field.Declaration.Type;
            var fieldName = field.Declaration.Variables.First().Identifier.Text;
            var fieldNameNew = char.ToUpper(fieldName[0]) + fieldName.Substring(1);

            sb.Append($"\n        {fieldName} = children[\"{fieldNameNew}\"].First() as {fieldType};");
        }

        sb.Append("\n    }\n}");

        return sb.ToString();
    }
}
