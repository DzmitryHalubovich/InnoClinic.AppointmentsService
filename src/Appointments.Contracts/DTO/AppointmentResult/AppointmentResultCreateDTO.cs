namespace Appointments.Contracts.DTO.Result;

public class AppointmentResultCreateDTO
{
    public Guid AppointmentId { get; set; }

    public string PatientBirthDate { get; set; }

    public string Complaints { get; set; }

    public string Conclusion { get; set; }

    public string Recommendations { get; set; }
}
