using WPR.Data.Base.Entities.Interfaces;

namespace WPR.Data.Base.Entities.EntityFramework;

public abstract class DeletedDbEntity : DbEntity, IDeletedEntity<int>
{
    public bool IsDeleted { get; set; }
}