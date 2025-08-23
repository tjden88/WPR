using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WPR.Repository.EfCore;

/// <summary>
/// Реализация составного репозитория для Entity Framework Core.
/// Автоматически обнаруживает связи между "Левой" и "Правой" сущностью
/// Репозиторий оборачивает два типа сущностей и позволяет выполнять запросы и управлять
/// их связями в указанном DbContext.
/// </summary>
/// <typeparam name="T">Тип левой сущности в составной связи.</typeparam>
/// <typeparam name="T1">Тип правой сущности в составной связи.</typeparam>
/// <typeparam name="TContext">Тип DbContext, используемого репозиторием.</typeparam>
/// <remarks>
/// Использует QueryTrackingBehavior из Entity Framework Core для управления настройками отслеживания сущностей.
/// Данная реализация регистрируется и используется через внедрение зависимостей в сценариях,
/// связанных с составными связями между двумя сущностями.
/// </remarks>
internal class EfCoreCompositeRepository<T, T1, TContext>(
    TContext db,
    QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.NoTracking)
    : EfCoreBaseCompositeRepository<T, T1>(db,
        CreateLeftKeySelector(db),
        CreateRightKeySelector(db),
        trackingBehavior)
    where T1 : class
    where T : class
    where TContext : DbContext
{
    private static Expression<Func<T, object>> CreateLeftKeySelector(DbContext db)
    {
        var leftEntity = db.Model.FindEntityType(typeof(T))
                         ?? throw new InvalidOperationException($"Тип {typeof(T).Name} не найден в модели EF.");
        var rightEntity = db.Model.FindEntityType(typeof(T1))
                          ?? throw new InvalidOperationException($"Тип {typeof(T1).Name} не найден в модели EF.");

        // Случай: T1 (Right) ссылается на T (Left) — у Left берём ключ принципала
        var fkRightToLeft = rightEntity.GetForeignKeys()
            .FirstOrDefault(fk => fk.PrincipalEntityType == leftEntity);
        if (fkRightToLeft is not null)
            return BuildKeySelector<T>(fkRightToLeft.PrincipalKey.Properties);

        // Случай: T (Left) ссылается на T1 (Right) — у Left берём FK свойства
        var fkLeftToRight = leftEntity.GetForeignKeys()
            .FirstOrDefault(fk => fk.PrincipalEntityType == rightEntity);
        if (fkLeftToRight is not null)
            return BuildKeySelector<T>(fkLeftToRight.Properties);

        throw new InvalidOperationException(
            $"Не удалось найти связь между {typeof(T).Name} и {typeof(T1).Name} в модели EF.");
    }

    private static Expression<Func<T1, object>> CreateRightKeySelector(DbContext db)
    {
        var leftEntity = db.Model.FindEntityType(typeof(T))
                         ?? throw new InvalidOperationException($"Тип {typeof(T).Name} не найден в модели EF.");
        var rightEntity = db.Model.FindEntityType(typeof(T1))
                          ?? throw new InvalidOperationException($"Тип {typeof(T1).Name} не найден в модели EF.");

        // Случай: T1 (Right) ссылается на T (Left) — у Right берём FK свойства
        var fkRightToLeft = rightEntity.GetForeignKeys()
            .FirstOrDefault(fk => fk.PrincipalEntityType == leftEntity);
        if (fkRightToLeft is not null)
            return BuildKeySelector<T1>(fkRightToLeft.Properties);

        // Случай: T (Left) ссылается на T1 (Right) — у Right берём ключ принципала
        var fkLeftToRight = leftEntity.GetForeignKeys()
            .FirstOrDefault(fk => fk.PrincipalEntityType == rightEntity);
        if (fkLeftToRight is not null)
            return BuildKeySelector<T1>(fkLeftToRight.PrincipalKey.Properties);

        throw new InvalidOperationException(
            $"Не удалось найти связь между {typeof(T).Name} и {typeof(T1).Name} в модели EF.");
    }

    private static Expression<Func<TEntity, object>> BuildKeySelector<TEntity>(IReadOnlyList<IProperty> properties)
        where TEntity : class
    {
        if (properties.Count == 0)
            throw new InvalidOperationException($"У типа {typeof(TEntity).Name} не найдены свойства ключа.");

        var param = Expression.Parameter(typeof(TEntity), "e");

        // Один ключ — просто боксим в object
        if (properties.Count == 1)
        {
            var body = Expression.Convert(CreatePropertyAccess(param, properties[0]), typeof(object));
            return Expression.Lambda<Func<TEntity, object>>(body, param);
        }

        // Составной ключ — создаём ValueTuple и боксим его в object
        var elementExprs = properties.Select(p => CreatePropertyAccess(param, p)).ToArray();
        var elementTypes = elementExprs.Select(pe => pe.Type).ToArray();

        var tupleType = GetValueTupleType(elementTypes.Length).MakeGenericType(elementTypes);
        var ctor = tupleType.GetConstructors().First(c => c.GetParameters().Length == elementTypes.Length);
        var newTuple = Expression.New(ctor, elementExprs);
        var boxed = Expression.Convert(newTuple, typeof(object));
        return Expression.Lambda<Func<TEntity, object>>(boxed, param);
    }

    private static Expression CreatePropertyAccess(ParameterExpression parameter, IProperty property)
    {
        // Есть CLR-свойство — обращаемся напрямую
        if (property.PropertyInfo is not null)
            return Expression.Property(parameter, property.PropertyInfo);

        // Теневое свойство — EF.Property<T>(e, "Name")
        var efProperty = typeof(EF).GetMethod(nameof(EF.Property), new[] { typeof(object), typeof(string) })!;
        var generic = efProperty.MakeGenericMethod(property.ClrType);
        return Expression.Call(generic, parameter, Expression.Constant(property.Name));
    }

    private static Type GetValueTupleType(int count) => count switch
    {
        2 => typeof(ValueTuple<,>),
        3 => typeof(ValueTuple<,,>),
        4 => typeof(ValueTuple<,,,>),
        5 => typeof(ValueTuple<,,,,>),
        6 => typeof(ValueTuple<,,,,,>),
        7 => typeof(ValueTuple<,,,,,,>),
        8 => typeof(ValueTuple<,,,,,,,>),
        _ => throw new NotSupportedException("Составные ключи с числом свойств больше 8 не поддерживаются.")
    };
}