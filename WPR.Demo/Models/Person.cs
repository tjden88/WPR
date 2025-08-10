namespace WPR.Demo.Models;

public abstract class Entity
{
    public int Id { get; set; }
}

public class Person(string Name, int Age) : Entity
{
    public string Name { get; init; } = Name;
    public int Age { get; init; } = Age;
}