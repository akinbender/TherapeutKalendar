namespace TherapeutKalendar.Shared.Models;

public abstract class Person
{
    public Person()
    {
    }
    public Person(string firstName, string lastName)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
    }

    public Guid Id { get; } = Guid.NewGuid();

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public List<Termin> Termine { get; set; } = new();
}