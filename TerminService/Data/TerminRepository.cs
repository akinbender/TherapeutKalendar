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
            SELECT t.id, 
                   t.start_time AS ""StartTime"", 
                   t.end_time AS ""EndTime"", 
                   t.status,
                   p.id, 
                   p.first_name AS ""FirstName"", 
                   p.last_name AS ""LastName"",
                   th.id, 
                   th.first_name AS ""FirstName"", 
                   th.last_name AS ""LastName""
            FROM termins t
            JOIN patients p ON t.patient_id = p.id
            JOIN therapists th ON t.therapist_id = th.id
            WHERE t.start_time >= @Start AND t.end_time <= @End";

        return await _connection.QueryAsync<Termin, Patient, Therapist, Termin>(
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

    public async Task<IEnumerable<Termin>> GetNextAvailableTerminsAsync(Guid therapistId, DateTime till)
    {
        var query = @"
            SELECT t.id, 
                   t.start_time AS ""StartTime"", 
                   t.end_time AS ""EndTime"", 
                   t.status,
                   p.id, 
                   p.first_name AS ""FirstName"", 
                   p.last_name AS ""LastName"",
                   th.id, 
                   th.first_name AS ""FirstName"", 
                   th.last_name AS ""LastName""
            FROM termins t
            JOIN patients p ON t.patient_id = p.id
            JOIN therapists th ON t.therapist_id = th.id
            WHERE t.therapist_id = @TherapistId
              AND t.start_time > NOW()
              AND t.start_time <= @Till
            ORDER BY t.start_time";
        return await _connection.QueryAsync<Termin, Patient, Therapist, Termin>(
            query,
            (termin, patient, therapist) => {
                termin.Patient = patient;
                termin.Therapist = therapist;
                return termin;
            },
            new { TherapistId = therapistId, Till = till },
            splitOn: "id,id");
    }
}