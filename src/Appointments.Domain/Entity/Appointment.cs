using System.Text.Json.Serialization;

namespace Appointments.Domain.Entity;

public class Appointment
{
    public Guid Id { get; set; }

    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public int SpecializationId { get; set; }

    [JsonPropertyName("specialization_name")]
    public string SpecializationName { get; set; }

    public Guid PatientId { get; set; }

    public Guid DoctorId { get; set; }

    public string OfficeId { get; set; } = null!;

    public string OfficeAddress { get; set; } = null!;

    public DateTime AppointmentDate { get; set; }

    public string PatientFullName { get; set; }

    public string DoctorFullName { get; set; }

    public string PatientEmail { get; set; } = null!;

    public bool IsApproved { get; set; }

    public bool IsNotificationSent { get; set; }
}
