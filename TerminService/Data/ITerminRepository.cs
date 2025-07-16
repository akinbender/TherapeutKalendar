using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TherapeutKalendar.Shared.Models;

namespace TerminService.Data;

public interface ITerminRepository
{
    Task<IEnumerable<Termin>> GetTermineAsync(DateTime start, DateTime end);
    Task<Termin> CreateTerminAsync(Termin termin);
    Task<bool> UpdateTerminAsync(Guid id, string status);
}