using System.ComponentModel.DataAnnotations;
using WPR.Data.Entities.Interfaces;

namespace WPR.Data.Entities;

public abstract class Entity<TKey> : IEntity<TKey> where TKey : notnull
{
    [Key]
    public TKey Id { get; set; } = default!;
}