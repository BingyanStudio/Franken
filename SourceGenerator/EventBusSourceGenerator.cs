using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Franken.SourceGenerator;

[Generator]
public class EventBusSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var eventBusClasses = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax,
                transform: (generatorSyntaxContext, _) =>
                {
                    var classDeclaration = (ClassDeclarationSyntax)generatorSyntaxContext.Node;
                    var classSymbol = generatorSyntaxContext.SemanticModel.GetDeclaredSymbol(classDeclaration);
                    return new { ClassDeclaration = classDeclaration, ClassSymbol = classSymbol };
                })
            .Where(t => t.ClassDeclaration is not null
                     && t.ClassDeclaration.HasAttribute("EventBus")
                     && t.ClassDeclaration.IsPartialClass())
            .Collect();

        var compilationAndClasses = context.CompilationProvider.Combine(eventBusClasses);

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
        StringBuilder sb = new();

        string className = classDeclaration.Identifier.Text, namesp = classSymbol.ContainingNamespace?.ToString() ?? string.Empty;
        var starting = namesp == string.Empty ? string.Empty : $"namespace {namesp};\n";

        // Reliances
        var usings = classDeclaration.GetUsingsInFile();
        foreach (var us in usings) sb.AppendLine($"using {us};");
        if (!usings.Contains("System")) sb.AppendLine("using System;");

        // Class Header & Singleton
        sb.Append($@"
{starting}
public partial class {className}
{{
    private static {className} instance;

    public static {className} Instance => instance;

    public override void _EnterTree() => instance = this;

    public override void _ExitTree() => instance = null;
");

        var signals = new List<(string SignalName, string DelegateName)>();

        // Signal Extension
        foreach (var delegateSyntax in classDeclaration.ChildNodes()
                                              .OfType<DelegateDeclarationSyntax>()
                                              .Where(n => n.HasAttribute("Signal")))
        {
            var delegateName = delegateSyntax.Identifier.Text;
            var signalName = delegateName.Replace("EventHandler", "");
            signals.Add((signalName, delegateName));

            var sendMethodName = $"Send{signalName}";

            var parameters = delegateSyntax.ParameterList.Parameters;
            var singalParams = string.Join(", ", parameters.Select(p => $"{p.Type} {p.Identifier.Text}"));
            var emitCallArgs = string.Join("", parameters.Select(p => $", {p.Identifier.Text}"));

            sb.Append($@"
    /// <summary>
    /// {sendMethodName}()
    /// </summary>
    public static void {sendMethodName}({singalParams}) => Instance.EmitSignal(SignalName.{signalName}{emitCallArgs});
    ");
        }

        sb.Append("\n}\n");

        // Extensions For Await
        sb.Append($@"
/// <summary>
/// 事件总线的 await 扩展方法
/// </summary>
public static class {className}Extensions
{{
");
        foreach (var (signalName, delegateName) in signals)
        {
            sb.Append($@"
    /// <summary>
    /// 等待 <see cref=""{className}.{delegateName}""/> 信号。
    /// 用法<code>await this.Await{signalName}()</code>;
    /// </summary>
    public static Godot.SignalAwaiter Await{signalName}(this Godot.GodotObject waiter)
    {{
        return waiter.ToSignal({className}.Instance, {className}.SignalName.{signalName});
    }}

");
        }

        sb.Append("\n}\n");

        return sb.ToString();
    }
}

