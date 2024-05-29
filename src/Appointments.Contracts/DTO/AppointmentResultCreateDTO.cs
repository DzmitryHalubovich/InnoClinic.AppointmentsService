namespace Appointments.Contracts.DTO;

public class AppointmentResultCreateDTO
{
    public Guid AppointmentId { get; set; }

    public string Complaints { get; set; }

    public string Conclusion { get; set; }

    public string Recomendations { get; set; }
}
