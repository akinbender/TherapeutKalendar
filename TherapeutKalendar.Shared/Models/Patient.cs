namespace TherapeutKalendar.Shared.Models;

public class Patient : Person
{
    public Patient()
    {
    }

    public Patient(string firstName, string lastName) : base(firstName, lastName)
    {
    }
}