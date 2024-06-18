using Appointments.Contracts;
using Appointments.Domain.Entity;
using Appointments.Domain.Interfaces;
using Appointments.Infrastructure.Data;
using Dapper;
using System.Text;

namespace Appointments.Infrastructure.Repositories;

public class AppointmentsRepository : IAppointmentsRepository
{
    public readonly AppointmentsDbContext _context;

    public AppointmentsRepository(AppointmentsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync(QueryParameters queryParameters)
    {
        StringBuilder query = new ("SELECT * FROM Appointments WHERE 1 = 1 ");

        if (queryParameters.DoctorId is not null)
        {
            query.Append("AND doctorId = @DoctorId ");
        }

        if (queryParameters.ServiceId is not null)
        {
            query.Append("AND serviceId = @ServiceId ");
        }

        if (queryParameters.PatientId is not null)
        {
            query.Append("AND patientId = @PatientId ");
        }

        if (queryParameters.OfficeId is not null)
        {
            query.Append("AND officeId = @OfficeId");
        }

        if (queryParameters.OnlyApproved is true)
        {
            query.Append("AND isapproved = true ");
        }

        using (var connection = _context.CreateConnection())
        {
            var appointments = await connection.QueryAsync<Appointment>(query.ToString(), queryParameters);

            return appointments;
        }
    }

    public async Task<IEnumerable<Appointment>> GetAllApprovedForNotitfication()
    {
        var query = "SELECT * FROM Appointments a " +
                    "WHERE a.IsApproved = true and a.NotificationIsSent = false";

        using (var connection = _context.CreateConnection())
        {
            var appointments = await connection.QueryAsync<Appointment>(query);

            return appointments;
        }
    }

    public async Task<Appointment?> GetByIdAsync(Guid id)
    {
        var query = "SELECT * FROM Appointments " +
                    "WHERE Id = @id";

        using (var connection = _context.CreateConnection())
        {
            var appointment = await connection.QuerySingleOrDefaultAsync<Appointment>(query, new { id });

            return appointment;
        }
    }

    public async Task<Guid> CreateAsync(Appointment appointment)
    {
        var query = "INSERT INTO Appointments (PatientId, DoctorId, ServiceId, OfficeId, SpecializationId, AppointmentDate, TimeSlot, PatientEmail)" +
                    "VALUES(@PatientId, @DoctorId, @ServiceId, @OfficeId, @SpecializationId, @AppointmentDate, @TimeSlot, @PatientEmail)" + "RETURNING Id;";

        using (var connection = _context.CreateConnection())
        {
            var createdAppointmentId = await connection.QuerySingleAsync<Guid>(query, appointment);

            return createdAppointmentId;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var query = "DELETE FROM Appointments " +
                    "WHERE Id = @id";

        using (var connection = _context.CreateConnection())
        {
            await connection.QueryAsync(query, new { id });
        }
    }

    public async Task UpdateAsync(Appointment appointment)
    {
        var query = "UPDATE Appointments " +
                    "SET doctorid = @DoctorId, specializationid = @SpecializationId, serviceid = @ServiceId, appointmentdate = @AppointmentDate, timeslot = @TimeSlot " +
                    "WHERE Id = @Id";

        using (var connection = _context.CreateConnection())
        {
            await connection.QueryAsync(query, 
                new { appointment.DoctorId, appointment.SpecializationId, 
                    appointment.ServiceId, appointment.AppointmentDate, appointment.TimeSlot, appointment.Id });
        }
    }

    public async Task ApproveAsync(Guid id)
    {
        var query = "UPDATE Appointments " +
                    "SET IsApproved = true " +
                    "WHERE Id = @id";

        using (var connection = _context.CreateConnection())
        {
            await connection.QueryAsync(query, new { id });
        }
    }

    public async Task DeleteAllForDeletedServiceAsync(int serviceId)
    {
        var query = "DELETE FROM Appointments " +
                    "WHERE ServiceId = @serviceId";

        using (var connection = _context.CreateConnection())
        {
            await connection.QueryAsync(query, new { serviceId });
        }
    }

    public async Task SetNotificationIsSent(IEnumerable<Appointment> appointments)
    {
        using (var connection = _context.CreateConnection())
        {
            var query = "UPDATE appointments SET NotificationIsSent = true WHERE Id = @Id";

            foreach (var appointment in appointments)
            {
                await connection.ExecuteAsync(query, new { appointment.Id });
            }
        }
    }
}
