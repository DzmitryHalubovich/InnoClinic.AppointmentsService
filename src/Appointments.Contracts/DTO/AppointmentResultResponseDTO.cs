namespace Appointments.Contracts.DTO;

public class AppointmentResultResponseDTO
{
    public Guid Id { get; set; }

    public Guid AppointmentId { get; set; }

    public string Complaints { get; set; }

    public string Conclusion { get; set; }

    public string Recomendations { get; set; }

    public AppointmentResponseDTO AppointmentResponseDTO { get; set; }
}
