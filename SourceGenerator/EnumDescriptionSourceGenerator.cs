using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Franken.SourceGenerator;

[Generator]
public class EnumDescriptionSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var enumDeclarations = context.SyntaxProvider
           .CreateSyntaxProvider( // TODO: 换掉内置的Description标签
                predicate: (syntaxNode, _) => syntaxNode is EnumDeclarationSyntax,
                transform: (generatorSyntaxContext, _) =>
                {
                    var enumDeclaration = (EnumDeclarationSyntax)generatorSyntaxContext.Node;
                    var enumSymbol = generatorSyntaxContext.SemanticModel.GetDeclaredSymbol(enumDeclaration) as INamedTypeSymbol;
                    return new { EnumDeclaration = enumDeclaration, EnumSymbol = enumSymbol };
                })
           .Where(t => t.EnumSymbol != null)
           .Collect();

        var compilationAndEnums = context.CompilationProvider.Combine(enumDeclarations);

        context.RegisterSourceOutput(compilationAndEnums, (sourceProductionContext, tuple) =>
        {
            foreach (var item in tuple.Right)
            {
                var enumDeclaration = item.EnumDeclaration;
                var enumSymbol = item.EnumSymbol;

                if (!enumSymbol.GetMembers("GetDescription").Any())
                {
                    var source = GenerateSourceCode(enumSymbol);
                    sourceProductionContext.AddSource($"{enumSymbol.GetFullName().Replace(".", "")}Descriptions.g.cs", SourceText.From(source, Encoding.UTF8));
                }
            }

        });
    }

    // 生成枚举描述扩展方法的代码
    private static string GenerateSourceCode(INamedTypeSymbol enumSymbol)
    {
        var enumName = enumSymbol.GetFullName();
        var namespaceName = enumSymbol.ContainingNamespace?.ToString() ?? string.Empty;

        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(namespaceName)) sb.AppendLine($"namespace {namespaceName};").AppendLine();
        sb.AppendLine($"public static partial class EnumExtensions");
        sb.AppendLine("{");
        sb.AppendLine($"    public static string GetDescription(this {enumName} value) =>");
        sb.AppendLine("        value switch");
        sb.AppendLine("        {");

        int cnt = 0;

        foreach (var member in enumSymbol.GetMembers().Where(m => m.Kind == SymbolKind.Field))
        {
            var description = member.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.Name == "DescriptionAttribute")
                ?.ConstructorArguments.FirstOrDefault().Value?.ToString()
                ?? member.Name;

            sb.AppendLine($"            {enumName}.{member.Name} => \"{description}\",");

            cnt++;
        }
        sb.AppendLine("            _ => string.Empty");
        sb.AppendLine("        };");
        sb.AppendLine("}");
        return sb.ToString();
    }
}
