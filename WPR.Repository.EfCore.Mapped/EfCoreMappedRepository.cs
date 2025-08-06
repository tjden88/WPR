using Mapster;
using Microsoft.EntityFrameworkCore;

namespace WPR.Repository.EfCore.Mapped;

public class EfCoreMappedRepository<TScr, TDest, TContext>(TContext Db) : EfCoreBaseMappedRepository<TScr, TDest>(Db) where TScr : class where TDest : class where TContext : DbContext
{

    protected override IQueryable<TDest> MappedItems => Db.Set<TScr>().ProjectToType<TDest>();

    protected override TScr MapToSource(TDest dest) => dest.Adapt<TScr>();

}