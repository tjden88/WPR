using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WPR.Repository.Abstractions;

namespace WPR.Repository.EfCore;

public static class Registrator
{

    /// <summary>
    /// Зарегистрировать репозиторий для сущности EF Core
    /// </summary>
    /// <typeparam name="TEntity">Сущность БД</typeparam>
    /// <typeparam name="TContext">Контекст данных приложения</typeparam>
    /// <param name="services"></param>
    /// <param name="registerMethod">Какой интерфейс зарегистрировать</param>
    public static IServiceCollection AddEfCoreRepository<TEntity, TContext>(this IServiceCollection services, RegisterMethod registerMethod = RegisterMethod.CrudRepository)
        where TEntity : class
        where TContext : DbContext
    {
        if (registerMethod is RegisterMethod.CrudRepository or RegisterMethod.Both)
            services.AddScoped<IRepository<TEntity>, EfCoreRepository<TEntity, TContext>>();

        if (registerMethod is RegisterMethod.ReadOnlyRepository or RegisterMethod.Both) 
            services.AddScoped<IReadOnlyRepository<TEntity>, EfCoreRepository<TEntity, TContext>>();

        return services;
    }


    /// <summary>
    /// Зарегистрировать репозиторий для сущности EF Core со своей реализацией
    /// </summary>
    /// <typeparam name="TEntity">Сущность БД</typeparam>
    /// <param name="services"></param>
    /// <param name="implementationType">Кастомная реализация репозитория</param>
    /// <param name="registerMethod">Какой интерфейс зарегистрировать</param>
    public static IServiceCollection AddEfCoreRepository<TEntity>(this IServiceCollection services, Type implementationType, RegisterMethod registerMethod = RegisterMethod.CrudRepository) where TEntity : class
    {
        if (registerMethod is RegisterMethod.CrudRepository or RegisterMethod.Both)
        {
            var repoInterface = typeof(IRepository<>).MakeGenericType(typeof(TEntity));
            services.AddScoped(repoInterface, implementationType);
        }

        if (registerMethod is RegisterMethod.ReadOnlyRepository or RegisterMethod.Both)
        {
            var repoInterface = typeof(IReadOnlyRepository<>).MakeGenericType(typeof(TEntity));
            services.AddScoped(repoInterface, implementationType);
        }

        return services;
    }


    /// <summary>
    /// Зарегистрировать репозитории для всех сущностей в контексте TContext, на основе объявленных публичных DbSet
    /// </summary>
    /// <typeparam name="TContext">Контекст данных приложения</typeparam>
    /// <param name="services"></param>
    /// <param name="registerMethod">Как зарегистрировать интерфейсы</param>
    public static IServiceCollection AddEfCoreRepositories<TContext>(this IServiceCollection services, RegisterMethod registerMethod = RegisterMethod.CrudRepository)
        where TContext : DbContext
    {
        var dbSetProperties = typeof(TContext)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

        foreach (var property in dbSetProperties)
        {
            var entityType = property.PropertyType.GetGenericArguments()[0];

            var implementationType = typeof(EfCoreRepository<,>).MakeGenericType(entityType, typeof(TContext));

            if (registerMethod is RegisterMethod.CrudRepository or RegisterMethod.Both)
            {
                var repoInterface = typeof(IRepository<>).MakeGenericType(entityType);
                services.AddScoped(repoInterface, implementationType);
            }

            if (registerMethod is RegisterMethod.ReadOnlyRepository or RegisterMethod.Both)
            {
                var repoInterface = typeof(IReadOnlyRepository<>).MakeGenericType(entityType);
                services.AddScoped(repoInterface, implementationType);
            }
        }

        return services;
    }
}