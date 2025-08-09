using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace WPR.Mvvm.SourceGen;

/// <summary>
/// Incremental source generator, который для каждого partial класса, наследующегося от ViewModel<T>,
/// генерирует приватные поля с атрибутом CommunityToolkit.Mvvm.ComponentModel.ObservableProperty
/// на основании публичных свойств модели T.
/// 
/// Поддерживает генерацию ObservableCollection<TItem> для свойств IEnumerable<TItem> (кроме string).
/// </summary>
[Generator]
public class ViewModelForModelGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Debugger.Launch(); // УБРАНО: вызывало зависание генератора при сборке

        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsCandidateClass(node),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null)!;

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, static (spc, source) =>
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
                        var hintName = $"{GetFullMetadataName(classSymbol)}.ViewModelFromModel.g.cs";
                        spc.AddSource(hintName.Replace('<', '_').Replace('>', '_'), SourceText.From(generated, Encoding.UTF8));
                    }
                }
                catch (Exception ex)
                {
                    var diag = Diagnostic.Create(new DiagnosticDescriptor(
                            "WPRSG0001",
                            "Generator error",
                            $"Ошибка в генераторе ViewModelForModel: {ex.Message}",
                            "ViewModelGenerator",
                            DiagnosticSeverity.Warning,
                            isEnabledByDefault: true),
                        Location.None);
                    spc.ReportDiagnostic(diag);
                }
            }
        });
    }

    #region Syntax helper

    // Проверяем синтаксический кандидат (быстрый фильтр).
    private static bool IsCandidateClass(SyntaxNode node)
    {
        if (node is ClassDeclarationSyntax cds)
        {
            // Быстрый фильтр: у класса должен быть base list (наследование или интерфейсы)
            return cds.BaseList != null;
        }

        return false;
    }

    // Дальше извлекаем семантическую информацию: нужен именно наследник ViewModel<T>
    private static MatchedClass? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDecl = (ClassDeclarationSyntax)context.Node;

        var semanticModel = context.SemanticModel;
        var classSymbol = semanticModel.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;
        if (classSymbol == null) return null;

        // Нас интересуют именно partial классы — чтобы можно было генерировать partial-член
        if (!classDecl.Modifiers.Any(m => m.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword)))
            return null;

        // Проверяем базовые типы — ищем ViewModel<T>
        foreach (var baseType in classSymbol.AllInterfaces.Concat([classSymbol.BaseType]).Where(t => t != null))
        {
            // Не используем interface — это просто быстрый обход. Будем проверять BaseType отдельно.
        }

        // Проверим сам непосредственный базовый тип (BaseType) и все типы в цепочке,
        // чтобы найти шаблон ViewModel<T> (неважно в каком пространстве имён).
        var baseNamed = classSymbol.BaseType;
        while (baseNamed != null)
        {
            if (baseNamed is {IsGenericType: true, Name: "ViewModel", TypeArguments.Length: 1})
            {
                var modelType = baseNamed.TypeArguments[0] as INamedTypeSymbol;
                return new MatchedClass
                {
                    ClassSymbol = classSymbol,
                    ModelType = modelType
                };
            }

            baseNamed = baseNamed.BaseType;
        }

        // Так же проверим, может класс реализует ViewModel<T> через интерфейс (маловероятно, но на всякий).
        // (опционально — пропускаем)

        return null;
    }

    #endregion

    #region Generation

    // Основная логика генерации одного файла для одного класса
    private static string GenerateFor(INamedTypeSymbol classSymbol, ITypeSymbol modelTypeSymbol, Compilation compilation)
    {
        // Получаем пространство имён для класса
        var ns = classSymbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : classSymbol.ContainingNamespace.ToDisplayString();

        var sb = new StringBuilder();

        // Заголовок файла
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("// Сгенерировано ViewModelForModelGenerator");
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.Collections.ObjectModel;");
        sb.AppendLine();

        if (!string.IsNullOrEmpty(ns))
        {
            sb.AppendLine($"namespace {ns}");
            sb.AppendLine("{");
        }

        // Нужна корректная декларация partial класса: включая type parameters и constraints
        // Собираем имя типа вместе с generic параметрами (например: public partial class Foo<TModel> : ViewModel<TModel>)
        var classDeclaration = GeneratePartialClassDeclaration(classSymbol);

        sb.AppendLine(classDeclaration);
        sb.AppendLine("{");

        // Получаем публичные свойства модели (instance, публичные, с getter и setter)
        // modelTypeSymbol может быть INamedTypeSymbol, или TypeParameterSymbol - поддерживаем только именованные типы
        if (modelTypeSymbol is not INamedTypeSymbol modelNamed)
        {
            // В сложных сценариях T мог быть тип-параметр; в таком случае мы не генерируем ничего
            sb.AppendLine("    // Не удалось разрешить тип модели T на этапе генерации — пропускаем генерацию свойств.");
        }
        else
        {
            // Перечисляем свойства модели
            var modelProperties = modelNamed.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(p => p is {DeclaredAccessibility: Accessibility.Public, IsStatic: false, GetMethod: not null, SetMethod: not null})
                .ToList();

            // Собираем имена уже существующих свойств/полей/членов в ViewModel, чтобы не генерировать дубли
            var existingMemberNames = new HashSet<string>(classSymbol.GetMembers().Select(m => m.Name));

            foreach (var prop in modelProperties)
            {
                // Пропускаем индексаторы
                if (prop.IsIndexer) continue;

                var propName = prop.Name;

                // Если свойство с таким именем уже есть в целевом ViewModel — пропускаем
                if (existingMemberNames.Contains(propName))
                {
                    sb.AppendLine($"    // Пропущено: свойство '{propName}' уже определено в ViewModel.");
                    continue;
                }

                // Решаем тип для генерации:
                // - Если тип реализует IEnumerable<TItem> (и не string), то генерируем ObservableCollection<TItem>
                // - Иначе — генерируем поле того же типа, что и модельное свойство
                var targetType = GetTargetGeneratedType(prop.Type, compilation);

                // Формируем имя поля: _{camelCase}
                var fieldName = "_" + ToCamelCase(propName);

                // Генерируем поле с атрибутом CommunityToolkit.Mvvm.ComponentModel.ObservableProperty
                // Полностью квалифицируем атрибут, чтобы не зависеть от using.
                sb.AppendLine("    /// <summary>");
                sb.AppendLine($"    /// Сгенерированное поле для свойства модели {modelNamed.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}.{propName}");
                sb.AppendLine("    /// Генерируется атрибутом [ObservableProperty], CommunityToolkit создаст публичное свойство.");
                sb.AppendLine("    /// </summary>");

                // Если целевой тип — generic, получаем его отображение в коде
                var targetTypeName = targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                // Если это ObservableCollection<...>, создаём инициализацию пустой коллекции,
                // чтобы избежать NRE и чтобы UI мог сразу привязаться.
                if (IsObservableCollectionTypeString(targetTypeName))
                {
                    sb.AppendLine($"    [global::CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]");
                    sb.AppendLine($"    private {targetTypeName} {fieldName} = new {targetTypeName}();");
                }
                else
                {
                    sb.AppendLine($"    [global::CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]");
                    sb.AppendLine($"    private {targetTypeName} {fieldName};");
                }

                sb.AppendLine();
            }
        }

        sb.AppendLine("}"); // конец partial class

        if (!string.IsNullOrEmpty(ns))
        {
            sb.AppendLine("}"); // конец namespace
        }

        return sb.ToString();
    }

    #endregion

    #region Utilities

    // Вспомогательный тип для передачи найденного класс+модель
    private record MatchedClass
    {
        public INamedTypeSymbol? ClassSymbol { get; set; }
        public ITypeSymbol? ModelType { get; set; }
    }

    // Получает корректную декларацию partial класса (имя + generic параметры + ограничения)
    // Пример: "public partial class PersonViewModel : WPR.Mvvm.ViewModels.ViewModel<Person>"
    private static string GeneratePartialClassDeclaration(INamedTypeSymbol classSymbol)
    {
        // Модификаторы: сохраняем публичность / internal / protected? — упростим: используем модификатор типа (public/internal)
        var accessibility = classSymbol.DeclaredAccessibility == Accessibility.Public ? "public"
            : classSymbol.DeclaredAccessibility == Accessibility.Internal ? "internal"
            : "public"; // на всякий случай

        // Имя типа без namespace
        var name = classSymbol.Name;

        // Generic параметры, если есть
        var typeParams = "";
        if (classSymbol.TypeParameters.Length > 0)
        {
            var names = string.Join(", ", classSymbol.TypeParameters.Select(tp => tp.Name));
            typeParams = $"<{names}>";
        }

        // Получаем bases — но мы не повторяем базовый тип (ViewModel<T>) — оставляем как есть в объявлении,
        // потому что partial класс уже наследует тот же базовый тип в исходнике пользователя.
        // Чтобы быть безопаснее, не пишем базовые типы — просто генерируем "partial class X"
        return $"    {accessibility} partial class {name}{typeParams}";
    }

    // Проверяет — строка содержит ObservableCollection<...> в fully-qualified представлении
    private static bool IsObservableCollectionTypeString(string s)
    {
        return s.Contains("System.Collections.ObjectModel.ObservableCollection");
    }

    // Преобразует Name -> camelCase (например: "FullName" -> "fullName")
    private static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        if (name.Length == 1) return name.ToLowerInvariant();
        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }

    // Возвращает корректный target type для генерации:
    // если propType реализует IEnumerable<TItem> (и не string) => ObservableCollection<TItem>
    // иначе => тот же тип propType
    private static ITypeSymbol GetTargetGeneratedType(ITypeSymbol propType, Compilation compilation)
    {
        // Если строка — это IEnumerable<char> фактически, но string мы исключаем
        if (propType.SpecialType == SpecialType.System_String)
            return propType;

        // Ищем интерфейс IEnumerable<T>
        var ienumerableOfT = compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1");

        if (ienumerableOfT == null) return propType;

        // Если propType реализует IEnumerable<T>, берем тип аргумента
        var implements = FindIEnumerableOfT(propType, ienumerableOfT);
        if (implements is INamedTypeSymbol {TypeArguments.Length: 1} named)
        {
            var itemType = named.TypeArguments[0];

            // Построим тип ObservableCollection<itemType>
            var obsColl = compilation.GetTypeByMetadataName("System.Collections.ObjectModel.ObservableCollection`1");
            if (obsColl != null)
            {
                // Создаём специализированный тип ObservableCollection<itemType>
                return obsColl.Construct(itemType);
            }
        }

        // Иначе — возвращаем исходный тип
        return propType;
    }

    // Рекурсивно ищем реализацию IEnumerable<T> в типе (включая generic, interfaces, base types)
    private static ITypeSymbol? FindIEnumerableOfT(ITypeSymbol typeSymbol, INamedTypeSymbol enumerableGeneric)
    {
        if (typeSymbol == null) return null;

        // Если это сам IEnumerable<T>
        if (typeSymbol is INamedTypeSymbol {IsGenericType: true} nts)
        {
            if (SymbolEqualityComparer.Default.Equals(nts.OriginalDefinition, enumerableGeneric))
                return nts;
        }

        // Проверяем все интерфейсы
        foreach (var iface in typeSymbol.AllInterfaces)
        {
            if (iface is INamedTypeSymbol {IsGenericType: true} iNamed &&
                SymbolEqualityComparer.Default.Equals(iNamed.OriginalDefinition, enumerableGeneric))
            {
                return iNamed;
            }
        }

        // Подъём по иерархии типов
        if (typeSymbol.BaseType != null)
            return FindIEnumerableOfT(typeSymbol.BaseType, enumerableGeneric);

        return null;
    }

    // Получить полное уникальное имя типа (namespace + имя + generic), для имени файла
    private static string GetFullMetadataName(INamedTypeSymbol type)
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
                    current = (ISymbol) nts.ContainingType ?? nts.ContainingNamespace;
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

    #endregion
}