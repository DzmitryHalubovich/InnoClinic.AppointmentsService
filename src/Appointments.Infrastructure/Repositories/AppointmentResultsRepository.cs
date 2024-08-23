using Appointments.Domain.Entity;
using Appointments.Domain.Interfaces;
using Appointments.Infrastructure.Data;
using Dapper;

namespace Appointments.Infrastructure.Repositories;

public class AppointmentResultsRepository : IAppointmentResultsRepository
{
    public readonly AppointmentsDbContext _context;

    public AppointmentResultsRepository(AppointmentsDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateAsync(AppointmentResult appointmentResult)
    {
        var query = "INSERT INTO appointmentResults (\"appointmentId\", " +
                                                    "\"patientBirthDate\", " +
                                                    "\"complaints\", " +
                                                    "\"conclusion\", " +
                                                    "\"recommendations\") " +
                    "VALUES(@AppointmentId, " +
                           "@PatientBirthDate, " +
                           "@Complaints, " +
                           "@Conclusion, " +
                           "@Recommendations) " +
                    "RETURNING id;";

        using var connection = _context.CreateConnection();
        
        var createdResultId = await connection.QuerySingleAsync<Guid>(query, appointmentResult);

        return createdResultId;
    }

    public async Task<AppointmentResult?> GetByIdAsync(Guid id)
    {
        var query = "SELECT * " +
                    "FROM appointmentResults " +
                    "WHERE id = @id ;";

        using var connection = _context.CreateConnection();
        
        var appointmentResult = await connection.QuerySingleOrDefaultAsync<AppointmentResult>(query, new { id });

        return appointmentResult;
    }

    public async Task UpdateAsync(AppointmentResult appointmentResult)
    {
        var query = "UPDATE appointmentResults SET \"complaints\" = @Complaints, " +
                                                  "\"patientBirthDate\" = @patientBirthDate, " +
                                                  "\"conclusion\" = @Conclusion, " +
                                                  "\"recommendations\" = @Recommendations " +
                    "WHERE Id = @Id";

        using var connection = _context.CreateConnection();
        
        await connection.QueryAsync(query, 
            new { 
                appointmentResult.Complaints, 
                appointmentResult.PatientBirthDate,
                appointmentResult.Conclusion, 
                appointmentResult.Recommendations, 
                appointmentResult.Id 
            });
    }

    public async Task DeleteAsync(Guid id)
    {
        var query = "DELETE from appointmentResults " +
                    "WHERE Id = @id";

        using var connection = _context.CreateConnection();

        await connection.QueryAsync(query, new { id });
    }
}
