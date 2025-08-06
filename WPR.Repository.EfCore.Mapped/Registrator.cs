using Microsoft.Extensions.DependencyInjection;
using WPR.Repository.Abstractions;

namespace WPR.Repository.EfCore.Mapped;

public static class Registrator
{

    /// <summary>
    /// Добавить репозиторий для получения и изменения сущностей другого типа (с преобразованием в тип БД и обратно) на основе Mapster Mapper
    /// </summary>
    /// <typeparam name="TSrc">Сущность БД</typeparam>
    /// <typeparam name="TDest">Будет зарегистрирован интерфейс IRepository для этого типа</typeparam>
    /// <typeparam name="TContext">Контекст БД для TSrc</typeparam>
    /// <param name="services"></param>
    /// <param name="registerMethod">Как зарегистрировать репозиторий</param>
    /// <returns></returns>
    public static IServiceCollection AddMappedEfCoreRepository<TSrc, TDest, TContext>(this IServiceCollection services, RegisterMethod registerMethod = RegisterMethod.CrudRepository) 
    {

        var implementationType = typeof(EfCoreMappedRepository<,,>).MakeGenericType(typeof(TSrc), typeof(TDest), typeof(TContext));

        if (registerMethod is RegisterMethod.CrudRepository or RegisterMethod.Both)
        {
            var repoInterface = typeof(IRepository<>).MakeGenericType(typeof(TDest));
            services.AddScoped(repoInterface, implementationType);
        }

        if (registerMethod is RegisterMethod.ReadOnlyRepository or RegisterMethod.Both)
        {
            var repoInterface = typeof(IReadOnlyRepository<>).MakeGenericType(typeof(TDest));
            services.AddScoped(repoInterface, implementationType);
        }

        return services;
    }
     
}