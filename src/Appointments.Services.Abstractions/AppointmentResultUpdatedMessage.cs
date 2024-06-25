namespace Appointments.Services.Abstractions;

public class AppointmentResultUpdatedMessage
{
    public Guid AppointmentResultId { get; set; }

    public string PatientFullName { get; set; }

    public string PatientEmail { get; set; }
}
