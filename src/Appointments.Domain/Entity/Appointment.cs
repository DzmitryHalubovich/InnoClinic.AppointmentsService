namespace Appointments.Domain.Entity;

public class Appointment
{
    public Guid Id { get; set; }

    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public int SpecializationId { get; set; }

    public string SpecializationName { get; set; } = null!;

    public Guid PatientId { get; set; }

    public string PatientFullName { get; set; } = null!;

    public string PatientEmail { get; set; } = null!;

    public Guid DoctorId { get; set; }

    public string DoctorFullName { get; set; } = null!;

    public string OfficeId { get; set; } = null!;

    public string OfficeAddress { get; set; } = null!;

    public DateTime AppointmentDate { get; set; }

    public bool IsApproved { get; set; }

    public bool IsNotificationSent { get; set; }
}
