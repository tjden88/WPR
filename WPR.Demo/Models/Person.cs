using System;

namespace WPR.Demo.Models;

public abstract class Entity
{
    public int Id { get; set; }
}

public class Person(string Name, int Age) : Entity
{
    public DateTime Created { get; } = DateTime.Now;
    public string Name { get; set; } = Name;
    public int Age { get; set; } = Age;
}