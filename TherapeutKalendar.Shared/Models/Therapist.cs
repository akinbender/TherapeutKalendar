using TherapeutKalendar.Shared.Enums;
namespace TherapeutKalendar.Shared.Models;

public class Therapist : Person
{
    public Therapist()
    {
    }
    public Therapist(string firstName, string lastName) : base(firstName, lastName, true)
    {
        
    }
    public TimeSpan DailyStartHour { get; set; } = new(8, 0, 0); // Default 8:00
    public TimeSpan DailyEndHour { get; set; } = new(17, 0, 0); // Default 17:00
    public TimeSpan LunchStart { get; set; } = new(12, 0, 0); // Default 12:00
    public TimeSpan LunchEnd { get; set; } = new(13, 0, 0); // Default 13:00

    public int DefaultAppointmentDurationInMinutes { get; set; } = 30; // Default 30 minutes
    public TherapistSpecialty Specialty { get; set; }
}
