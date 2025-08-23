using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WPR.Repository.Abstractions;

namespace WPR.Repository.EfCore;

/// <summary>
/// Базовая реализация составного репозитория для Entity Framework Core.
/// Репозиторий оборачивает два типа сущностей и позволяет выполнять запросы и управлять
/// их связями в указанном DbContext.
/// </summary>
/// <typeparam name="T">Тип левой сущности в составной связи.</typeparam>
/// <typeparam name="T1">Тип правой сущности в составной связи.</typeparam>
/// <remarks>
/// Использует QueryTrackingBehavior из Entity Framework Core для управления настройками отслеживания сущностей.
/// Данная реализация регистрируется и используется через внедрение зависимостей в сценариях,
/// связанных с составными связями между двумя сущностями.
/// </remarks>
public abstract class EfCoreBaseCompositeRepository<T, T1>(
    DbContext db,
    Expression<Func<T, object>> leftKey,
    Expression<Func<T1, object>> rightKey,
    QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.NoTracking) : ICompositeRepository<T, T1>
    where T : class
    where T1 : class
{
    
    // Применить Where к IQueryable<аноним>, подставив Left/Right в предикат (l, r) => ...
    private static IQueryable ApplyPairWhere(
        IQueryable pairs,
        Expression<Func<T, T1, bool>> predicate)
    {
        var anonType = pairs.ElementType;
        var p = Expression.Parameter(anonType, "p");
        var left = Expression.Property(p, "Left");
        var right = Expression.Property(p, "Right");

        var body = new ReplaceParametersVisitor(new Dictionary<ParameterExpression, Expression>
        {
            { predicate.Parameters[0], left },
            { predicate.Parameters[1], right }
        }).Visit(predicate.Body)!;

        var funcType = typeof(Func<,>).MakeGenericType(anonType, typeof(bool));
        var lambda = Expression.Lambda(funcType, body, p);

        var whereMi = typeof(Queryable).GetMethods()
            .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
            .MakeGenericMethod(anonType);

        var result = (IQueryable)whereMi.Invoke(null, [pairs, lambda])!;
        return result;
    }
    
    // Применить Select к IQueryable<аноним>, подставив Left/Right в selector (l, r) => TResult
    private static IQueryable<TResult> ApplyPairSelect<TResult>(
        IQueryable pairs,
        Expression<Func<T, T1, TResult>> selector)
    {
        var anonType = pairs.ElementType;
        var p = Expression.Parameter(anonType, "p");
        var left = Expression.Property(p, "Left");
        var right = Expression.Property(p, "Right");

        var body = new ReplaceParametersVisitor(new Dictionary<ParameterExpression, Expression>
        {
            { selector.Parameters[0], left },
            { selector.Parameters[1], right }
        }).Visit(selector.Body)!;

        var funcType = typeof(Func<,>).MakeGenericType(anonType, typeof(TResult));
        var lambda = Expression.Lambda(funcType, body, p);

        var selectMi = typeof(Queryable).GetMethods()
            .First(m => m.Name == "Select"
                        && m.GetParameters().Length == 2)
            .MakeGenericMethod(anonType, typeof(TResult));

        var result = (IQueryable<TResult>)selectMi.Invoke(null, [pairs, lambda])!;
        return result;
    }
    
    private IQueryable<TResult> ResultQuery<TResult>(
        Expression<Func<T, T1, TResult>> selector,
        Expression<Func<T, T1, bool>>? predicate = null,
        Func<IQueryable<TResult>, IOrderedQueryable<TResult>>? orderBy = null)
    {
        var leftQuery = db.Set<T>().AsTracking(trackingBehavior);
        var rightQuery = db.Set<T1>().AsTracking(trackingBehavior);

        // Join в анонимный тип
        IQueryable pairs = leftQuery.Join(
            rightQuery,
            leftKey,
            rightKey,
            (l, r) => new { Left = l, Right = r });

        // Фильтрация по паре ПОСЛЕ Join (если задана)
        if (predicate is not null)
            pairs = ApplyPairWhere(pairs, predicate);

        // Проекция к TResult
        var projected = ApplyPairSelect(pairs, selector);

        // Сортировка (после проекции)
        if (orderBy is not null)
            projected = orderBy(projected);

        return projected;
    }

    public async Task<List<TResult>> GetAsync<TResult>(
        Expression<Func<T, T1, TResult>> selector,
        Expression<Func<T, T1, bool>>? predicate = null,
        Func<IQueryable<TResult>, IOrderedQueryable<TResult>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        var resultQuery = ResultQuery(
            selector,
            predicate,
            orderBy);

        return await resultQuery.ToListAsync(cancellationToken);
    }
    
    public async Task<List<T>> GetLeftsAsync(
        Expression<Func<T, T1, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        var resultQuery = ResultQuery(
            (l, r) => l,
            predicate,
            orderBy: null);

        var dist = resultQuery.Distinct();
        if (orderBy is not null)
            dist = orderBy(dist);
        return await dist.ToListAsync(cancellationToken);
    }
    
    public async Task<List<T1>> GetRightsAsync(
        Expression<Func<T, T1, bool>>? predicate = null,
        Func<IQueryable<T1>, IOrderedQueryable<T1>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        var resultQuery = ResultQuery(
            (l, r) => r,
            predicate,
            orderBy: null);

        var dist = resultQuery.Distinct();
        if (orderBy is not null)
            dist = orderBy(dist);
        return await dist.ToListAsync(cancellationToken);
    }

    private sealed class ReplaceParametersVisitor(IReadOnlyDictionary<ParameterExpression, Expression> map)
        : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) =>
            map.TryGetValue(node, out var replacement) ? replacement : base.VisitParameter(node);

    }
}