using System.ComponentModel.DataAnnotations;
using WPR.Data.Entities.Interfaces;

namespace WPR.Data.Entities;

public class NamedEntity : Entity, INamedEntity
{
    [Required]
    public string Name { get; set; }
}