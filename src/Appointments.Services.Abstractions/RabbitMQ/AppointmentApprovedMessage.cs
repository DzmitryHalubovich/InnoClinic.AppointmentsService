namespace Appointments.Services.Abstractions.RabbitMQ;

public class AppointmentApprovedMessage
{
    public Guid AppointmentId { get; set; }

    public string PatientEmail { get; set; }

    public DateTime AppointmentDate { get; set; }
}
