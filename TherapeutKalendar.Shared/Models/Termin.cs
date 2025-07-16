using TherapeutKalendar.Shared.Enums;
namespace TherapeutKalendar.Shared.Models;

public class Termin
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TerminStatus Status { get; set; } = TerminStatus.Created;
    public Guid PatientId { get; set; }
    public Guid TherapistId { get; set; }

    public Person? Patient { get; set; }
    public Person? Therapist { get; set; }
}