using System.ComponentModel.DataAnnotations;

namespace Appointments.Contracts.DTO;

public class AppointmentResponseDTO
{
    public Guid Id { get; set; }

    public Guid PatientId { get; set; }

    public Guid DoctorId { get; set; }

    public int ServiceId { get; set; }

    public string OfficeId { get; set; }

    public int SpecializationId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public int TimeSlot { get; set; }

    public bool IsApproved { get; set; }
}
