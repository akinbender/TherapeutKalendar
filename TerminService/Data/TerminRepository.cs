using Dapper;
using Npgsql;
using TherapeutKalendar.Shared.Models;

namespace TerminService.Data;

public class TerminRepository(NpgsqlConnection connection) : ITerminRepository
{
    private readonly NpgsqlConnection _connection = connection;

    public async Task<IEnumerable<Termin>> GetTermineAsync(DateTime start, DateTime end)
    {
        var query = @"
            SELECT t.id, t.start_time, t.end_time, t.status,
                   p.id, p.first_name, p.last_name, p.is_therapist,
                   th.id, th.first_name, th.last_name, th.is_therapist
            FROM termins t
            JOIN persons p ON t.patient_id = p.id
            JOIN persons th ON t.therapist_id = th.id
            WHERE t.start_time >= @Start AND t.end_time <= @End";
        
        return await _connection.QueryAsync<Termin, Person, Person, Termin>(
            query,
            (termin, patient, therapist) => 
            {
                termin.Patient = patient;
                termin.Therapist = therapist;
                return termin;
            },
            new { Start = start, End = end },
            splitOn: "id,id");
    }

    public async Task<Termin> CreateTerminAsync(Termin termin)
    {
        var query = @"
            INSERT INTO termins (start_time, end_time, status, patient_id, therapist_id)
            VALUES (@StartTime, @EndTime, @Status, @PatientId, @TherapistId)
            RETURNING id";
        
        var id = await _connection.ExecuteScalarAsync<Guid>(query, new {
            StartTime = termin.StartTime,
            EndTime = termin.EndTime,
            Status = termin.Status.ToString(),
            PatientId = termin.PatientId,
            TherapistId = termin.TherapistId
        });
        
        termin.Id = id;
        return termin;
    }

    public async Task<bool> UpdateTerminAsync(Guid id, string status)
    {
        var query = @"
            UPDATE termins
            SET status = @Status
            WHERE id = @Id";
        
        var affected = await _connection.ExecuteAsync(query, new { Id = id, Status = status });
        return affected > 0;
    }
}