using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WPR.Repository.Abstractions;

namespace WPR.Repository.EfCore;

/// <summary>
/// Базовая реализация составного репозитория для EF Core.
/// Работает ТОЛЬКО с анонимными типами при Join/GroupJoin и умеет переключаться
/// между INNER (includeUnmatched = false) и LEFT (includeUnmatched = true).
/// </summary>
/// <typeparam name="T">Первая сущность</typeparam>
/// <typeparam name="T1">Вторая сущность</typeparam>
public abstract class EfCoreBaseCompositeRepository<T, T1>(
    DbContext db,
    Expression<Func<T, object>> leftKey,
    Expression<Func<T1, object>> rightKey,
    QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.NoTracking)
    : ICompositeRepository<T, T1> // как в твоём интерфейсе
    where T : class
    where T1 : class
{
    /// <summary>
    /// Собрать пары (Left, Right) анонимного типа в зависимости от режима.
    /// </summary>
    private IQueryable BuildPairs(bool includeUnmatched)
    {
        var leftQuery = db.Set<T>().AsTracking(trackingBehavior);
        var rightQuery = db.Set<T1>().AsTracking(trackingBehavior);

        if (includeUnmatched)
        {
            // LEFT JOIN: берём все Left, Right может быть null
            return leftQuery
                .GroupJoin(
                    rightQuery,
                    leftKey,
                    rightKey,
                    (l, rs) => new { Left = l, Rights = rs })
                .SelectMany(
                    x => x.Rights.DefaultIfEmpty(),
                    (x, r) => new { x.Left, Right = r });
        }

        // INNER JOIN: только совпавшие пары
        return leftQuery.Join(
            rightQuery,
            leftKey,
            rightKey,
            (l, r) => new { Left = l, Right = r });
    }

    // Применить Where к IQueryable<аноним>, подставив Left/Right в предикат (l, r) => ...
    private static IQueryable ApplyPairWhere(IQueryable pairs, Expression<Func<T, T1, bool>> predicate)
    {
        var anonType = pairs.ElementType;
        var p = Expression.Parameter(anonType, "p");
        var left = Expression.Property(p, "Left");
        var right = Expression.Property(p, "Right");

        var body = new ReplaceParametersVisitor(new Dictionary<ParameterExpression, Expression>
        {
            { predicate.Parameters[0], left  },
            { predicate.Parameters[1], right }
        }).Visit(predicate.Body)!;

        var funcType = typeof(Func<,>).MakeGenericType(anonType, typeof(bool));
        var lambda = Expression.Lambda(funcType, body, p);

        var whereMi = typeof(Queryable).GetMethods()
            .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
            .MakeGenericMethod(anonType);

        return (IQueryable)whereMi.Invoke(null, new object[] { pairs, lambda })!;
    }

    // Применить Select к IQueryable<аноним>, подставив Left/Right в selector (l, r) => TResult
    private static IQueryable<TResult> ApplyPairSelect<TResult>(IQueryable pairs, Expression<Func<T, T1, TResult>> selector)
    {
        var anonType = pairs.ElementType;
        var p = Expression.Parameter(anonType, "p");
        var left = Expression.Property(p, "Left");
        var right = Expression.Property(p, "Right");

        var body = new ReplaceParametersVisitor(new Dictionary<ParameterExpression, Expression>
        {
            { selector.Parameters[0], left  },
            { selector.Parameters[1], right }
        }).Visit(selector.Body)!;

        var funcType = typeof(Func<,>).MakeGenericType(anonType, typeof(TResult));
        var lambda = Expression.Lambda(funcType, body, p);

        var selectMi = typeof(Queryable).GetMethods()
            .First(m => m.Name == "Select" && m.GetParameters().Length == 2)
            .MakeGenericMethod(anonType, typeof(TResult));

        return (IQueryable<TResult>)selectMi.Invoke(null, new object[] { pairs, lambda })!;
    }

    /// <summary>
    /// Общая «сборка» запроса: пары -> where (если есть) -> select -> orderBy (если есть)
    /// </summary>
    private IQueryable<TResult> ResultQuery<TResult>(
        Expression<Func<T, T1, TResult>> selector,
        Expression<Func<T, T1, bool>>? predicate,
        Func<IQueryable<TResult>, IOrderedQueryable<TResult>>? orderBy,
        bool includeUnmatched)
    {
        IQueryable pairs = BuildPairs(includeUnmatched);

        if (predicate is not null)
            pairs = ApplyPairWhere(pairs, predicate);

        var projected = ApplyPairSelect(pairs, selector);

        if (orderBy is not null)
            projected = orderBy(projected);

        return projected;
    }

    // ===== Реализация интерфейса (как в твоём ICompositeRepository) =====

    public async Task<List<TResult>> GetAsync<TResult>(
        Expression<Func<T, T1, TResult>> selector,
        Expression<Func<T, T1, bool>>? predicate = null,
        Func<IQueryable<TResult>, IOrderedQueryable<TResult>>? orderBy = null,
        bool includeUnmatched = false,
        CancellationToken cancellationToken = default)
    {
        var query = ResultQuery(selector, predicate, orderBy, includeUnmatched);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<T>> GetLeftsAsync(
        Expression<Func<T, T1, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool includeUnmatched = false,
        CancellationToken cancellationToken = default)
    {
        var query = ResultQuery((l, r) => l, predicate, null, includeUnmatched).Distinct();
        if (orderBy is not null) query = orderBy(query);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<T1>> GetRightsAsync(
        Expression<Func<T, T1, bool>>? predicate = null,
        Func<IQueryable<T1>, IOrderedQueryable<T1>>? orderBy = null,
        bool includeUnmatched = false,
        CancellationToken cancellationToken = default)
    {
        // Если includeUnmatched = true, Right может быть null — отфильтруем перед Distinct,
        // чтобы не тащить null'ы в материализацию.
        var query = ResultQuery((l, r) => r, predicate, null, includeUnmatched)
            .Where(x => x != null) // EF переведёт в IS NOT NULL
            .Distinct();

        if (orderBy is not null) query = orderBy(query);
        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Заменяет параметры исходной лямбды на произвольные выражения (Left/Right).
    /// Никаких Invoke — всё остаётся переводимым в SQL.
    /// </summary>
    private sealed class ReplaceParametersVisitor(IReadOnlyDictionary<ParameterExpression, Expression> map) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) =>
            map.TryGetValue(node, out var replacement) ? replacement : base.VisitParameter(node);
    }
}
