namespace Appointments.Domain.Entity;

public class AppointmentResult
{
    public Guid Id { get; set; }

    public Guid AppointmentId { get; set; }

    public string Complaints { get; set; }

    public string Conclusion { get; set; }

    public string Recommendations { get; set; }

    public DateTime AppointmentDate { get; set; }
}
