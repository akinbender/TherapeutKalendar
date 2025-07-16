using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TherapeutKalendar.Shared.Enums;
using TherapeutKalendar.Shared.Models;

namespace TerminService.Data;

public class TherapistRepository : ITherapistRepository
{
    private readonly NpgsqlConnection _connection;

    public TherapistRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<Therapist?> GetByIdAsync(Guid id)
    {
        const string query = @"
            SELECT id, first_name, last_name, 
                   daily_start_hour, daily_end_hour, 
                   lunch_start, lunch_end, 
                   default_appointment_duration_minutes, specialty
            FROM therapists
            WHERE id = @Id";

        return await _connection.QueryFirstOrDefaultAsync<Therapist>(query, new { Id = id });
    }

    public async Task<IEnumerable<Therapist>> SearchByNameAsync(string name)
    {
        const string query = @"
            SELECT id, first_name, last_name, 
                   daily_start_hour, daily_end_hour, 
                   lunch_start, lunch_end, 
                   default_appointment_duration_minutes, specialty
            FROM therapists
            WHERE first_name ILIKE @SearchPattern OR last_name ILIKE @SearchPattern";

        return await _connection.QueryAsync<Therapist>(query, 
            new { SearchPattern = $"%{name}%" });
    }

    public async Task<IEnumerable<Therapist>> SearchBySpecialtyAsync(TherapistSpecialty specialty)
    {
        const string query = @"
            SELECT id, first_name, last_name, 
                   daily_start_hour, daily_end_hour, 
                   lunch_start, lunch_end, 
                   default_appointment_duration_minutes, specialty
            FROM therapists
            WHERE specialty = @Specialty";

        return await _connection.QueryAsync<Therapist>(query, 
            new { Specialty = specialty.ToString() });
    }

    // Additional useful methods
    public async Task<IEnumerable<Therapist>> GetAllAsync()
    {
        const string query = "SELECT * FROM therapists";
        return await _connection.QueryAsync<Therapist>(query);
    }

    public async Task<TimeSpan> GetDefaultAppointmentDurationAsync(Guid therapistId)
    {
        const string query = @"
            SELECT default_appointment_duration_minutes
            FROM therapists
            WHERE id = @TherapistId";
            
        var minutes = await _connection.ExecuteScalarAsync<int>(query, 
            new { TherapistId = therapistId });
            
        return TimeSpan.FromMinutes(minutes);
    }
}