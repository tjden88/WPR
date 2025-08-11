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
/// Incremental source generator, который для каждого partial класса, наследующегося от ObservableModel<T>,
/// генерирует приватные свойства CommunityToolkit.Mvvm.ComponentModel.ObservableProperty
/// на основании публичных модели T.
/// 
/// Поддерживает генерацию ObservableCollection<TItem> для свойств IEnumerable<TItem>.
/// </summary>
[Generator]
public class ObservableModelGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //Debugger.Launch(); // При локальной отладке генераторов удобно ставить

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
                        var hintName = $"{GetFullMetadataName(classSymbol)}.ObservableModelFromModel.g.cs";
                        spc.AddSource(hintName.Replace('<', '_').Replace('>', '_'),
                            SourceText.From(generated, Encoding.UTF8));
                    }
                }
                catch (Exception ex)
                {
                    var diag = Diagnostic.Create(new DiagnosticDescriptor(
                            "WPRSG0001",
                            "Generator error",
                            $"Ошибка в генераторе ObservableModelForModel: {ex.Message}",
                            "ObservableModelGenerator",
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

    // Дальше извлекаем семантическую информацию: нужен именно наследник ObservableModel<T>
    private static MatchedClass? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDecl = (ClassDeclarationSyntax) context.Node;

        var semanticModel = context.SemanticModel;
        var classSymbol = semanticModel.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;
        if (classSymbol == null) return null;

        // Нас интересуют именно partial классы — чтобы можно было генерировать partial-член
        if (!classDecl.Modifiers.Any(m => m.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword)))
            return null;

        // Проверяем базовые типы — ищем ObservableModel<T>
        foreach (var baseType in classSymbol.AllInterfaces.Concat([classSymbol.BaseType]).Where(t => t != null))
        {
            // Не используем interface — это просто быстрый обход. Будем проверять BaseType отдельно.
        }

        // Проверим сам непосредственный базовый тип (BaseType) и все типы в цепочке,
        // чтобы найти шаблон ObservableModel<T> (неважно в каком пространстве имён).
        var baseNamed = classSymbol.BaseType;
        while (baseNamed != null)
        {
            if (baseNamed is {IsGenericType: true, Name: "ObservableModel", TypeArguments.Length: 1})
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

        // Так же проверим, может класс реализует ObservableModel<T> через интерфейс (маловероятно, но на всякий).
        // (опционально — пропускаем)

        return null;
    }

    #endregion

    #region Generation

    // Основная логика генерации одного файла для одного класса
    private static string GenerateFor(INamedTypeSymbol classSymbol, ITypeSymbol modelTypeSymbol,
        Compilation compilation)
    {
        // Пространство имён класса (если есть).
        var ns = classSymbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : classSymbol.ContainingNamespace.ToDisplayString();
        var sb = new StringBuilder();

        // Имя Protected поля для хранения модели
        const string modelFieldName = "Model";

        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("// Сгенерировано ObservableModelForModelGenerator");
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.Collections.ObjectModel;");
        sb.AppendLine();

        if (!string.IsNullOrEmpty(ns))
        {
            sb.AppendLine($"namespace {ns}");
            sb.AppendLine("{");
        }

        // Генерируем объявление partial-класса (мы не повторяем base-тип — предполагаем, что исходный файл уже содержит его)
        var classDeclaration = GeneratePartialClassDeclaration(classSymbol);
        sb.AppendLine(classDeclaration);
        sb.AppendLine("{");

        // Если модель не удалось разрешить на этапе компиляции — выходим с комментарием
        if (modelTypeSymbol is not INamedTypeSymbol modelNamed)
        {
            sb.AppendLine("    // Не удалось разрешить тип модели T во время генерации. Генерация пропущена.");
            sb.AppendLine("}"); // конец class

            if (!string.IsNullOrEmpty(ns))
                sb.AppendLine("}"); // конец namespace

            return sb.ToString();
        }


        // Собираем instance-свойства модели
        var modelProperties = GetAllModelProperties(modelNamed);

        // Список уже существующих членов ObservableModel (имена), чтобы не генерировать дубли
        var existingMemberNames = new HashSet<string>(classSymbol.GetMembers().Select(m => m.Name));


        foreach (var prop in modelProperties)
        {
            if (prop.IsIndexer) continue;

            var propName = prop.Name;
            if (existingMemberNames.Contains(propName))
            {
                sb.AppendLine($"    // Пропущено: свойство '{propName}' уже определено в {classSymbol.Name}.");
                continue;
            }

            // Выясняем целевой тип для генерации (ObservableCollection<TItem> для IEnumerable<TItem> кроме string)
            var targetTypeSymbol = GetTargetGeneratedType(prop.Type, compilation);
            var targetTypeName = targetTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            // Комментарий в коде
            sb.AppendLine("    /// <summary>");
            sb.AppendLine(
                $"    /// Сгенерированное свойство для модели {modelNamed.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}.{propName}");
            sb.AppendLine("    /// Сгенерировано ObservableModelGenerator.");
            sb.AppendLine("    /// </summary>");

            // Имя приватного поля
            string setterFieldName;

            // Коллекция — создаём ObservableCollection<TItem>()
            var isCollection = IsObservableCollectionTypeSymbol(targetTypeSymbol);

            if (isCollection)
            {
                setterFieldName = "_" + ToCamelCase(propName);

                // Инициализируем коллекцию из свойства модели, если свойство модели не null
                sb.AppendLine($"    private {targetTypeName} {setterFieldName};");
            }
            else
                setterFieldName = $"{modelFieldName}.{propName}";


            sb.AppendLine();

            // Генерируем публичное свойство
            sb.AppendLine($"    public {targetTypeName} {propName}");
            sb.AppendLine("    {");
            
            if (isCollection)
                sb.AppendLine($"        get => {setterFieldName} ??= {modelFieldName}.{propName} is null ? null : new {targetTypeName}({modelFieldName}.{propName});");
            else
                sb.AppendLine($"        get => {setterFieldName} ;");

            if (prop.SetMethod is
                {DeclaredAccessibility: Accessibility.Public, IsInitOnly: false}) // Если не public - пропускаем сеттер
            {
                // генерируем сеттер, использующий SetProperty<TModel, T>(T oldValue, T newValue, IEqualityComparer<T> comparer, TModel model, Action<TModel, T> callback
                if (isCollection)
                {
                    sb.AppendLine($"        set");
                    sb.AppendLine($"        {{");
                    sb.AppendLine($"            if (SetProperty(ref {setterFieldName}, value) && value == null)");
                    sb.AppendLine($"                {modelFieldName}.{propName} = null;");
                    sb.AppendLine($"        }}");
                }
                else
                    sb.AppendLine(
                        $"        set => SetProperty({setterFieldName}, value, {modelFieldName}, (m, v) => m.{propName} = v);");
            }

            sb.AppendLine("    }");
            sb.AppendLine();
        }


        // Оператор преобразования модели — генерируем только если есть конструктор (T model)
        var hasSingleModelCtor = classSymbol.InstanceConstructors.Any(ctor =>
            !ctor.IsStatic &&
            ctor.Parameters.Length == 1 &&
            SymbolEqualityComparer.Default.Equals(ctor.Parameters[0].Type, modelTypeSymbol));

        if (hasSingleModelCtor)
        {
            sb.AppendLine("    /// <summary>");
            sb.AppendLine(
                $"    /// Оператор неявного преобразования модели {modelNamed.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}");
            sb.AppendLine("    /// Сгенерировано ObservableModelGenerator.");
            sb.AppendLine("    /// </summary>");
            
            sb.AppendLine(
                $"    public static implicit operator {classSymbol.Name}({modelTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} model)");
            sb.AppendLine("    {");
            sb.AppendLine($"        if (model is null) return null;");
            sb.AppendLine($"        var vm = new {classSymbol.Name}(model);");
            sb.AppendLine($"        return vm;");
            sb.AppendLine("    }");
        }
        else
        {
            sb.AppendLine($"    // Пропущено: оператор неявного преобразования не сгенерирован, так как отсутствует конструктор {classSymbol.Name}({modelNamed.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} model).");
        }


        sb.AppendLine("}"); // конец class

        if (!string.IsNullOrEmpty(ns))
            sb.AppendLine("}"); // конец namespace

        return sb.ToString();
    }


    // Проверка, является ли целевой тип ObservableCollection<...> (по symbol)
    private static bool IsObservableCollectionTypeSymbol(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol {IsGenericType: true} named)
        {
            return named.ConstructedFrom.ToDisplayString() == "System.Collections.ObjectModel.ObservableCollection<T>";
        }

        return false;
    }

    // Получаем все публичные instance-свойства модели, включая унаследованные
    private static List<IPropertySymbol> GetAllModelProperties(INamedTypeSymbol modelTypeSymbol)
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

                // Если уже добавили свойство с таким именем — пропускаем (приоритет у самого нижнего в иерархии)
                if (!seenNames.Add(prop.Name))
                    continue;

                result.Add(prop);
            }

            current = current.BaseType;
        }

        return result;
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
    // Пример: "public partial class PersonObservableModel : WPR.Mvvm.ObservableModels.ObservableModel<Person>"
    private static string GeneratePartialClassDeclaration(INamedTypeSymbol classSymbol)
    {
        // Модификаторы: сохраняем публичность / internal / protected? — упростим: используем модификатор типа (public/internal)
        var accessibility = classSymbol.DeclaredAccessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Internal => "internal",
            _ => "public"
        }; // на всякий случай

        // Имя типа без namespace
        var name = classSymbol.Name;

        // Generic параметры, если есть
        var typeParams = "";
        if (classSymbol.TypeParameters.Length > 0)
        {
            var names = string.Join(", ", classSymbol.TypeParameters.Select(tp => tp.Name));
            typeParams = $"<{names}>";
        }

        // Получаем bases — но мы не повторяем базовый тип (ObservableModel<T>) — оставляем как есть в объявлении,
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