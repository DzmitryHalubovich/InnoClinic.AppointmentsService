namespace Appointments.Contracts.DTO.Result;

public class AppointmentResultBaseDTO
{
    public DateTime PatientBirthDate { get; set; }

    public string PatientFirstName { get; set; }

    public string? PatientMiddleName { get; set; }

    public string PatientLastName { get; set; }

    public string DoctorFirstName { get; set; }

    public string? DoctorMiddleName { get; set; }

    public string DoctorLastName { get; set; }

    public string DoctorSpecialization { get; set; }

    public string ServiceName { get; set; }

    public string Complaints { get; set; }

    public string Conclusion { get; set; }

    public string Recommendations { get; set; }

    public string PatientEmail { get; set; }

    public Guid AppointmentId { get; set; }

    public DateTime AppointmentDate { get; set; }
}
