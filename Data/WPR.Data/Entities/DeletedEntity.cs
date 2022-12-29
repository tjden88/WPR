using WPR.Data.Entities.Interfaces;

namespace WPR.Data.Entities;

public class DeletedEntity : Entity, IDeletedEntity
{
    public bool IsDeleted { get; set; }
}