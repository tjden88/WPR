using System;
using System.Collections.Generic;

namespace WPR.Demo.Models;

public abstract class Entity
{
    public int Id { get; set; }
}

public class Person(string Name, int Age) : Entity
{
    public DateTime Created { get; } = DateTime.Now;
    public string Name { get; set; } = Name;
    
    public string Description { get; set; } 
    public int Age { get; set; } = Age;
    
    public int? ParentId { get; set; }
    
    public Person Parent { get; set; }
    
    public ICollection<string> Roles { get; set; } = [];
    public HashSet<Person> Persons { get; set; } = [];
}