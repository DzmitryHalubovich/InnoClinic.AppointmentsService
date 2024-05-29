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

    public async Task<Guid> CreateAsync(AppointmentResult appointment)
    {
        var query = "INSERT INTO AppointmentResults (AppointmentId, Complaints, Conclusion, Recomendations) " +
                    "VALUES(@AppointmentId, @Complaints, @Conclusion, @Recomendations) RETURNING Id;";

        using (var connection = _context.CreateConnection())
        {
            var createdResultId = await connection.QuerySingleAsync<Guid>(query, appointment);

            return createdResultId;
        }
    }

    public async Task<AppointmentResult?> GetByIdAsync(Guid id)
    {
        var query = "SELECT * " +
                    "FROM AppointmentResults ar " +
                    "INNER JOIN Appointments a on a.Id = ar.AppointmentId " +
                    $"WHERE ar.Id = '{id}' ;";

        using (var connection = _context.CreateConnection())
        {
            var appointmentResult = await connection.QueryAsync<AppointmentResult, Appointment, AppointmentResult>(
            query, (result, appointment) =>
            {
                result.Appointment = appointment;

                return result;
            },
            splitOn: "Id");

            return appointmentResult.FirstOrDefault();
        }
    }

    public async Task UpdateAsync(AppointmentResult appointmentResult)
    {
        var query = "UPDATE AppointmentResults " +
                    "SET Complaints = @Complaints, Conclusion = @Conclusion, Recomendations = @Recommendations " +
                    "WHERE Id = @Id";

        using (var connection = _context.CreateConnection())
        {
            await connection.QueryAsync(query, 
                new { appointmentResult.Complaints, appointmentResult.Conclusion, appointmentResult.Recomendations, Id = appointmentResult.Id });
        }
    }
}
