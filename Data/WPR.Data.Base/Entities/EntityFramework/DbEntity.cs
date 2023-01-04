using System.ComponentModel.DataAnnotations;
using WPR.Data.Base.Entities.Interfaces;

namespace WPR.Data.Base.Entities.EntityFramework;

public abstract class DbEntity : IEntity<int>
{
    [Key]
    public int Id { get; set; }
}