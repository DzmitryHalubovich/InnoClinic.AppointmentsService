namespace Appointments.Contracts.DTO.Appointment;

public class AppointmentCreateDTO
{
    public DateTime AppointmentDate { get; set; }

    public int ServiceId { get; set; }

    public string ServiceName { get; set; }

    public string OfficeId { get; set; }

    public int SpecializationId { get; set; }

    public Guid PatientId { get; set; }

    public string PatientFullName { get; set; }

    public string PatientEmail { get; set; }

    public Guid DoctorId { get; set; }

    public string DoctorFullName { get; set; }
}
