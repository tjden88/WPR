namespace WPR.Demo.Models
{
    public class Person(string Name, int Age)
    {
        public string Name { get; set; } = Name;
        public int Age { get; set; } = Age;

        public void Deconstruct(out string Name, out int Age)
        {
            Name = this.Name;
            Age = this.Age;
        }
    }
}
