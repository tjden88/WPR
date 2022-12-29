using System.ComponentModel.DataAnnotations;
using WPR.Data.Base.Entities.Interfaces;

namespace WPR.Data.Base.Entities;

public abstract class Entity<TKey> : IEntity<TKey> where TKey : notnull
{
    [Key]
    public TKey Id { get; set; } = default!;
}