using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace WPR.Mvvm.SourceGen;

// Базовый абстрактный генератор для классов, наследующихся от generic-базы с 1 типовым аргументом.
// Наследнику нужно:
// 1) указать имя целевого базового типа (TargetBaseTypeName), например "ObservableModel";
// 2) реализовать GenerateFor(...) — сформировать исходник для конкретного класса + типа модели.
public abstract class BaseGeneric1Generator : IIncrementalGenerator
{
    // Имя базового типа, от которого должен наследоваться целевой класс (без пространства имён), например: "ObservableModel"
    protected abstract string TargetBaseTypeName { get; }

    // Требовать, чтобы исходный класс был partial
    protected virtual bool RequirePartialClass => true;

    // Суффикс в имени генерируемого файла (можно переопределить)
    protected virtual string HintSuffix => TargetBaseTypeName;

    // Префикс для Diagnostic.Id (можно переопределить)
    protected virtual string DiagnosticIdPrefix => "GEN";

    // Основная генерация для конкретного совпадения (класс-VM и тип модели)
    protected abstract string GenerateFor(INamedTypeSymbol classSymbol, ITypeSymbol modelTypeSymbol, Compilation compilation);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsCandidateClass(node),
                transform: (ctx, _) => GetSemanticTarget(ctx, RequirePartialClass, TargetBaseTypeName))
            .Where(static m => m is not null)!;

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, (spc, source) =>
        {
            var compilation = source.Left;
            var matched = source.Right;

            foreach (var item in matched)
            {
                if (item is null) continue;

                var classSymbol = item.ClassSymbol;
                var modelType = item.ModelType;
                if (classSymbol == null || modelType == null) continue;

                try
                {
                    var generated = GenerateFor(classSymbol, modelType, compilation);
                    if (!string.IsNullOrWhiteSpace(generated))
                    {
                        var hintName = $"{GetFullMetadataName(classSymbol)}.{HintSuffix}.g.cs";
                        spc.AddSource(hintName.Replace('<', '_').Replace('>', '_'),
                            SourceText.From(generated, Encoding.UTF8));
                    }
                }
                catch (Exception ex)
                {
                    var diag = Diagnostic.Create(new DiagnosticDescriptor(
                            $"{DiagnosticIdPrefix}0001",
                            "Generator error",
                            $"Ошибка в генераторе {GetType().Name}: {ex.Message}",
                            GetType().Name,
                            DiagnosticSeverity.Warning,
                            isEnabledByDefault: true),
                        Location.None);
                    spc.ReportDiagnostic(diag);
                }
            }
        });
    }

    private static bool IsCandidateClass(SyntaxNode node)
    {
        if (node is ClassDeclarationSyntax cds)
            return cds.BaseList != null;
        return false;
    }

    private static MatchedClass? GetSemanticTarget(GeneratorSyntaxContext context, bool requirePartial, string targetBaseTypeName)
    {
        var classDecl = (ClassDeclarationSyntax)context.Node;

        var semanticModel = context.SemanticModel;
        var classSymbol = semanticModel.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;
        if (classSymbol == null) return null;

        if (requirePartial && !classDecl.Modifiers.Any(m => m.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword)))
            return null;

        // Ищем в цепочке базовых типов generic с именем targetBaseTypeName и ровно 1 типовым аргументом
        var baseNamed = classSymbol.BaseType;
        while (baseNamed != null)
        {
            if (baseNamed is { IsGenericType: true, Name: var name, TypeArguments.Length: 1 } && name == targetBaseTypeName)
            {
                var modelType = baseNamed.TypeArguments[0] as ITypeSymbol;
                return new MatchedClass
                {
                    ClassSymbol = classSymbol,
                    ModelType = modelType
                };
            }

            baseNamed = baseNamed.BaseType;
        }

        return null;
    }

    protected record MatchedClass
    {
        public INamedTypeSymbol? ClassSymbol { get; set; }
        public ITypeSymbol? ModelType { get; set; }
    }

    // ========== Защищённые вспомогательные методы для использования в наследниках ==========

    protected static string GeneratePartialClassDeclaration(INamedTypeSymbol classSymbol)
    {
        var accessibility = classSymbol.DeclaredAccessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Internal => "internal",
            _ => "public"
        };

        var name = classSymbol.Name;

        var typeParams = "";
        if (classSymbol.TypeParameters.Length > 0)
        {
            var names = string.Join(", ", classSymbol.TypeParameters.Select(tp => tp.Name));
            typeParams = $"<{names}>";
        }

        return $"    {accessibility} partial class {name}{typeParams}";
    }

    protected static List<IPropertySymbol> GetAllModelProperties(INamedTypeSymbol modelTypeSymbol)
    {
        var result = new List<IPropertySymbol>();
        var seenNames = new HashSet<string>(StringComparer.Ordinal);

        var current = modelTypeSymbol;

        while (current != null && current.SpecialType != SpecialType.System_Object)
        {
            foreach (var prop in current.GetMembers().OfType<IPropertySymbol>())
            {
                if (prop.IsStatic) continue;
                if (prop.DeclaredAccessibility != Accessibility.Public) continue;
                if (prop.GetMethod == null) continue;
                if (prop.IsIndexer) continue;

                if (!seenNames.Add(prop.Name))
                    continue;

                result.Add(prop);
            }

            current = current.BaseType;
        }

        return result;
    }

    protected static bool IsObservableCollectionTypeSymbol(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol { IsGenericType: true } named)
            return named.ConstructedFrom.ToDisplayString() == "WPR.Mvvm.Controls.ObservableCollectionEx<T>";

        return false;
    }

    protected static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        if (name.Length == 1) return name.ToLowerInvariant();
        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }

    protected static ITypeSymbol GetTargetGeneratedType(ITypeSymbol propType, Compilation compilation)
    {
        if (propType.SpecialType == SpecialType.System_String)
            return propType;

        var ienumerableOfT = compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1");
        if (ienumerableOfT == null) return propType;

        var implements = FindIEnumerableOfT(propType, ienumerableOfT);
        if (implements is INamedTypeSymbol { TypeArguments.Length: 1 } named)
        {
            var itemType = named.TypeArguments[0];

            var obsColl = compilation.GetTypeByMetadataName("WPR.Mvvm.Controls.ObservableCollectionEx`1");
            if (obsColl != null)
                return obsColl.Construct(itemType);
        }

        return propType;
    }

    protected static ITypeSymbol? FindIEnumerableOfT(ITypeSymbol typeSymbol, INamedTypeSymbol enumerableGeneric)
    {
        if (typeSymbol == null) return null;

        if (typeSymbol is INamedTypeSymbol { IsGenericType: true } nts)
        {
            if (SymbolEqualityComparer.Default.Equals(nts.OriginalDefinition, enumerableGeneric))
                return nts;
        }

        foreach (var iface in typeSymbol.AllInterfaces)
        {
            if (iface is INamedTypeSymbol { IsGenericType: true } iNamed &&
                SymbolEqualityComparer.Default.Equals(iNamed.OriginalDefinition, enumerableGeneric))
            {
                return iNamed;
            }
        }

        if (typeSymbol.BaseType != null)
            return FindIEnumerableOfT(typeSymbol.BaseType, enumerableGeneric);

        return null;
    }

    protected static string GetFullMetadataName(INamedTypeSymbol type)
    {
        var parts = new Stack<string>();
        ISymbol? current = type;

        while (current != null)
        {
            switch (current)
            {
                case INamedTypeSymbol nts:
                    var name = nts.Name + (nts.TypeParameters.Length > 0 ? $"`{nts.TypeParameters.Length}" : "");
                    parts.Push(name);
                    current = (ISymbol)nts.ContainingType ?? nts.ContainingNamespace;
                    break;

                case INamespaceSymbol ns:
                    if (!ns.IsGlobalNamespace)
                        parts.Push(ns.Name);
                    current = ns.ContainingNamespace;
                    break;

                default:
                    current = current.ContainingSymbol;
                    break;
            }
        }

        return string.Join(".", parts);
    }
}