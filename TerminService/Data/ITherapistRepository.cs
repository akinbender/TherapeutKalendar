using TherapeutKalendar.Shared.Models;
using TherapeutKalendar.Shared.Enums;

namespace TerminService.Data;

public interface ITherapistRepository
{               
    //Task<IEnumerable<Therapist>> GetAllAsync();
    Task<Therapist?> GetByIdAsync(Guid id);
    Task<IEnumerable<Therapist>> SearchByNameAsync(string name);
    Task<IEnumerable<Therapist>> SearchBySpecialtyAsync(TherapistSpecialty specialty);
}