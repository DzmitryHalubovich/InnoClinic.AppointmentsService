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
        var query = "INSERT INTO AppointmentResults (AppointmentId, Complaints, Conclusion, Recommendations, AppointmentDate) " +
                    "VALUES(@AppointmentId, @Complaints, @Conclusion, @Recommendations, @AppointmentDate) RETURNING Id;";

        using (var connection = _context.CreateConnection())
        {
            var createdResultId = await connection.QuerySingleAsync<Guid>(query, appointmentResult);

            return createdResultId;
        }
    }

    public async Task<AppointmentResult?> GetByIdAsync(Guid id)
    {
        var query = "SELECT Id, AppointmentId, Complaints, Conclusion, Recommendations, AppointmentDate " +
                    "FROM AppointmentResults ar " +
                    $"WHERE ar.Id = '{id}' ;";

        using (var connection = _context.CreateConnection())
        {
            var appointmentResult = await connection.QuerySingleOrDefaultAsync<AppointmentResult>(query, new { id });

            return appointmentResult;
        }
    }

    public async Task UpdateAsync(AppointmentResult appointmentResult)
    {
        var query = "UPDATE AppointmentResults " +
                    "SET Complaints = @Complaints, Conclusion = @Conclusion, Recommendations = @Recommendations " +
                    "WHERE Id = @Id";

        using (var connection = _context.CreateConnection())
        {
            await connection.QueryAsync(query, 
                new { appointmentResult.Complaints, appointmentResult.Conclusion, appointmentResult.Recommendations, Id = appointmentResult.Id });
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var query = "DELETE from AppointmentResults " +
                    "WHERE Id = @id";

        using (var connection = _context.CreateConnection())
        {
            await connection.QueryAsync(query, new { id });
        }
    }
}
