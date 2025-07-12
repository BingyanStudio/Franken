using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Franken.SourceGenerator;

internal static class SymbolExtensions
{
    internal static bool DirectlyInheritsFrom(this ClassDeclarationSyntax node, string name)
        => node.BaseList != null && node.BaseList.Types.Any(t => t.ToString() == name);

    internal static bool IsPartialClass(this ClassDeclarationSyntax node)
        => node.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));

    internal static IEnumerable<string> GetUsingsInFile(this ClassDeclarationSyntax node)
        => node.SyntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>().Select(u => u.Name.ToString());

    internal static string GetNamespace(this SyntaxNode node)
    {
        var current = node;
        while (current is not NamespaceDeclarationSyntax)
        {
            current = current.Parent;
            if (current == null) return string.Empty;
        }
        return (current as NamespaceDeclarationSyntax).Name.ToString();
    }

    internal static bool HasAttribute(this MemberDeclarationSyntax node, string name)
        => node.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == name));

    internal static AttributeSyntax GetAttribute(this MemberDeclarationSyntax node, string name)
    {
        foreach (var al in node.AttributeLists)
            foreach (var a in al.Attributes)
                if (a.Name.ToString() == name) return a;
        return null;
    }

    internal static T GetUnique<T>(this AttributeSyntax node) where T : ExpressionSyntax
        => node.ArgumentList.Arguments.Select(a => a.Expression).OfType<T>().First();

    internal static string GetClass(this FieldDeclarationSyntax node)
        => (node.Parent as ClassDeclarationSyntax).Identifier.Text;

    internal static string GetFullName(this INamedTypeSymbol symbol)
    {
        var name = symbol.Name;
        while (symbol.ContainingType != null)
            name = (symbol = symbol.ContainingType).Name + "." + name;
        return name;
    }
}