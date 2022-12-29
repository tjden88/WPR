namespace WPR.Data.Entities.Interfaces;

public interface IDeletedEntity : IEntity
{
    bool IsDeleted { get; set; }
}