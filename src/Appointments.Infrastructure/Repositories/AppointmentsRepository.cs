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
        StringBuilder query = new ("SELECT * FROM appointments WHERE 1 = 1 ");

        if (queryParameters.DoctorId is not null)
        {
            query.Append("AND \"doctorId\" = @DoctorId ");
        }

        if (queryParameters.ServiceId is not null)
        {
            query.Append("AND \"serviceId\" = @ServiceId ");
        }

        if (queryParameters.PatientId is not null)
        {
            query.Append("AND \"patientId\" = @PatientId ");
        }

        if (queryParameters.OfficeId is not null)
        {
            query.Append("AND \"officeId\" = @OfficeId");
        }

        if (queryParameters.OnlyApproved is true)
        {
            query.Append("AND \"isApproved\" = true ");
        }

        using var connection = _context.CreateConnection();
            
        var appointments = await connection.QueryAsync<Appointment>(query.ToString(), queryParameters);

        return appointments;
    }

    public async Task<IEnumerable<Appointment>> GetAllApprovedForNotitficationAsync()
    {
        var query = "SELECT * FROM appointments " +
                    "WHERE \"isApproved\" = true and \"isNotificationSent\" = false";

        using var connection = _context.CreateConnection();
        
        var appointments = await connection.QueryAsync<Appointment>(query);

        return appointments;
    }

    public async Task<Appointment?> GetByIdAsync(Guid id)
    {
        var query = "SELECT * FROM appointments " +
                    "WHERE id = @id";

        using var connection = _context.CreateConnection();
        
        var appointment = await connection.QuerySingleOrDefaultAsync<Appointment>(query, new { id });

        return appointment;
    }

    public async Task<Guid> CreateAsync(Appointment appointment)
    {
        var parameters = new { appointment.ServiceId, appointment.ServiceName, appointment.SpecializationId, appointment.SpecializationName, appointment.PatientId, appointment.DoctorId, appointment.OfficeId, appointment.OfficeAddress, appointment.AppointmentDate, appointment.PatientFullName, appointment.DoctorFullName, appointment.PatientEmail };
        var query = "INSERT INTO appointments (\"serviceId\", " +
                                              "\"serviceName\", " +
                                              "\"specializationId\", " +
                                              "\"specializationName\", " +
                                              "\"patientId\", " +
                                              "\"doctorId\", " +
                                              "\"officeId\", " +
                                              "\"officeAddress\", " +
                                              "\"appointmentDate\", " +
                                              "\"patientFullName\", " +
                                              "\"doctorFullName\", " +
                                              "\"patientEmail\", " +
                                              "\"isApproved\", " +
                                              "\"isNotificationSent\")" +
                    "VALUES(@ServiceId, " +
                    "@ServiceName, " +
                    "@SpecializationId, " +
                    "@SpecializationName, " +
                    "@PatientId, " +
                    "@DoctorId, " +
                    "@OfficeId, " +
                    "@OfficeAddress, " +
                    "@AppointmentDate, " +
                    "@PatientFullName, " +
                    "@DoctorFullName, " +
                    "@PatientEmail, " +
                    "false, " +
                    "false) " + 
                    "RETURNING id;";

        using var connection = _context.CreateConnection();

        var createdAppointmentId = await connection.QuerySingleAsync<Guid>(query, parameters);

        return createdAppointmentId;
    }

    public async Task DeleteAsync(Guid id)
    {
        var query = "DELETE FROM appointments " +
                    "WHERE id = @id";

        using var connection = _context.CreateConnection();
        
        await connection.QueryAsync(query, new { id });
    }

    public async Task UpdateAsync(Appointment appointment)
    {
        var query = "UPDATE appointments " +
                    "SET \"serviceId\" = @ServiceId, " +
                        "\"serviceName\" = @ServiceName, " +
                        "\"specializationId\" = @SpecializationId, " +
                        "\"specializationName\" = @SpecializationName, " +
                        "\"patientFullName\" = @PatientFullName, " +
                        "\"patientEmail\" = @PatientEmail, " +
                        "\"doctorId\" = @DoctorId, " +
                        "\"doctorFullName\" = @DoctorFullName, " +
                        "\"officeId\" = @OfficeId, " + 
                        "\"officeAddress\" = @OfficeAddress, " +
                        "\"appointmentDate\" = @AppointmentDate " +
                    "WHERE Id = @Id";

        using var connection = _context.CreateConnection();
        
        await connection.QueryAsync(query, 
            new { 
                appointment.ServiceId, 
                appointment.ServiceName,
                appointment.SpecializationId,
                appointment.SpecializationName,
                appointment.PatientFullName,
                appointment.PatientEmail,
                appointment.DoctorId,
                appointment.DoctorFullName,
                appointment.OfficeId,
                appointment.OfficeAddress,
                appointment.AppointmentDate,
                appointment.Id
            });
    }

    public async Task ApproveAsync(Guid id)
    {
        var query = "UPDATE appointments " +
                    "SET \"isApproved\" = true " +
                    "WHERE id = @id";

        using var connection = _context.CreateConnection();
        
        await connection.QueryAsync(query, new { id });
    }

    public async Task DeleteAllForDeletedServiceAsync(int serviceId)
    {
        var query = "DELETE FROM appointments " +
                    "WHERE \"serviceId\" = @serviceId";

        using var connection = _context.CreateConnection();
        
        await connection.QueryAsync(query, new { serviceId });
    }

    public async Task SetNotificationIsSentAsync(IEnumerable<Appointment> appointments)
    {
        var query = "UPDATE appointments SET \"isNotificationSent\" = true WHERE id = @Id";

        using var connection = _context.CreateConnection();

        foreach (var appointment in appointments)
        {
            await connection.ExecuteAsync(query, new { appointment.Id });
        }
    }
}
