using Mapster;
using Microsoft.EntityFrameworkCore;

namespace WPR.Repository.EfCore.Mapped;

public class EfCoreMappedRepository<TSrc, TDest, TContext>(TContext Db) : EfCoreBaseMappedRepository<TSrc, TDest>(Db) where TSrc : class where TDest : class where TContext : DbContext
{

    protected override IQueryable<TDest> MappedItems => Db.Set<TSrc>().ProjectToType<TDest>();

    protected override TSrc MapToSource(TDest dest) => dest.Adapt<TSrc>();

    protected override void MapBackAfterAdded(TDest item, TSrc src) => src.Adapt(item);
}