using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Franken.SourceGenerator;

[Generator]
public class TransitionSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax,
                transform: (generatorSyntaxContext, _) =>
                {
                    var classDeclaration = (ClassDeclarationSyntax)generatorSyntaxContext.Node;
                    var classSymbol = generatorSyntaxContext.SemanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;
                    return new { ClassDeclaration = classDeclaration, ClassSymbol = classSymbol };
                })
            .Where(t => t.ClassDeclaration is not null
                     && t.ClassDeclaration.HasAttribute("TransitionTarget")
                     && t.ClassDeclaration.IsPartialClass())
            .Collect();

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations);

        context.RegisterSourceOutput(compilationAndClasses, (sourceProductionContext, tuple) =>
        {
            foreach (var item in tuple.Right)
            {
                var classDeclaration = item.ClassDeclaration;
                var classSymbol = item.ClassSymbol;

                if (!classSymbol.GetMembers("target").Any())
                {
                    var (partialSource, extensionSource) = GenerateSourceCode(classDeclaration, classSymbol);
                    sourceProductionContext.AddSource($"{classSymbol.Name}.g.cs", SourceText.From(partialSource, Encoding.UTF8));
                    sourceProductionContext.AddSource($"{classSymbol.Name}ExtensionForTransition.g.cs", SourceText.From(extensionSource, Encoding.UTF8));
                }
            }
        });
    }

    private static (string, string) GenerateSourceCode(ClassDeclarationSyntax classDeclaration, INamedTypeSymbol classSymbol)
    {
        StringBuilder partialBuilder = new(), extensionBuilder = new(), ctorBuilder = new(), initBuilder = new(), processBuilder = new();

        string className = classDeclaration.Identifier.Text, namesp = classSymbol.ContainingNamespace?.ToString() ?? string.Empty;
        var targetType = classDeclaration.GetAttribute("TransitionTarget").GetUnique<TypeOfExpressionSyntax>().Type;
        var starting = namesp == string.Empty ? string.Empty : $"namespace {namesp};\n";

        partialBuilder.Clear();
        extensionBuilder.Clear();
        ctorBuilder.Clear();
        initBuilder.Clear();
        processBuilder.Clear();

        foreach (var us in classDeclaration.GetUsingsInFile())
        {
            partialBuilder.AppendLine($"using {us};");
            extensionBuilder.AppendLine($"using {us};");
        }

        partialBuilder.Append($@"
{starting}
public partial class {className} : Transition
{{
    private readonly {targetType} target;
                ");

        extensionBuilder.Append($@"
{starting}
public static class {targetType}ExtensionsForTransition
{{
                ");

        ctorBuilder.Append($@"
    public {className}({targetType} target) : base()
    {{
        this.target = target;
                ").AppendLine();

        initBuilder.Append($@"
    protected override void Init()
    {{
        base.Init();
                ").AppendLine();

        processBuilder.Append($@"
    protected override void Process(float delta)
    {{
        base.Process(delta);

        if (target == null)
        {{
            Drop();
            return;
        }}
                ").AppendLine();
        foreach (var field in classDeclaration.ChildNodes()
                                              .OfType<FieldDeclarationSyntax>()
                                              .Where(n => n.HasAttribute("TransitionField")))
        {
            if (field.GetClass() != className) continue;

            var fieldType = field.Declaration.Type;
            var fieldName = field.Declaration.Variables.First().Identifier.Text;
            var fieldNameNew = char.ToUpper(fieldName[0]) + fieldName.Substring(1);
            var relative = field.GetAttribute("TransitionField").GetUnique<LiteralExpressionSyntax>().Token.ValueText;

            partialBuilder.Append($@"
    private {fieldType} from{fieldNameNew}, to{fieldNameNew};

    private partial void Init{fieldNameNew}({targetType} target);
    private partial void Process{fieldNameNew}({targetType} target, {fieldType} delta);

    public {className} From{fieldNameNew}({fieldType} {fieldName})
    {{
        this.{fieldName} = from{fieldNameNew} = {fieldName};
        return this;
    }}

    public {className} To{fieldNameNew}({fieldType} {fieldName})
    {{
        to{fieldNameNew} = {fieldName};
        return this;
    }}
                    ");

            extensionBuilder.Append($@"
    public static {className} From{fieldNameNew}(this {targetType} target, {fieldType} {fieldName}) => new {className}(target).From{fieldNameNew}({fieldName});
    public static {className} To{fieldNameNew}(this {targetType} target, {fieldType} {fieldName}) => new {className}(target).From{fieldNameNew}(target.{relative}).To{fieldNameNew}({fieldName});
    public static {className} Add{fieldNameNew}(this {targetType} target, {fieldType} {fieldName}) => new {className}(target).From{fieldNameNew}(target.{relative}).To{fieldNameNew}(target.{relative} + {fieldName});
                    ");

            ctorBuilder.AppendLine($"        this.{fieldName} = from{fieldNameNew} = to{fieldNameNew} = target.{relative};");
            initBuilder.AppendLine($"        Init{fieldNameNew}(target);");
            processBuilder.AppendLine($"        Process{fieldNameNew}(target, delta * (to{fieldNameNew} - from{fieldNameNew}));");
        }

        ctorBuilder.AppendLine("    }");
        partialBuilder.Append(ctorBuilder.ToString());
        initBuilder.AppendLine("    }");
        partialBuilder.Append(initBuilder.ToString());
        processBuilder.AppendLine("    }");
        partialBuilder.Append(processBuilder.ToString());

        var ending = "\n}";

        partialBuilder.Append(ending);
        extensionBuilder.Append(ending);

        return (partialBuilder.ToString(), extensionBuilder.ToString());
    }
}
