namespace TherapeutKalendar.Shared.Models;

public abstract class Person
{
    public Person()
    {
    }
    public Person(string firstName, string lastName, bool isTherapist = false)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        IsTherapist = isTherapist;
    }

    public Guid Id { get; } = Guid.NewGuid();
    public string FirstName { get; } = string.Empty;
    public string LastName { get; } = string.Empty;
    public bool IsTherapist { get; set; } = false;
    public List<Termin> Termine { get; set; } = new();
}