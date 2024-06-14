namespace Appointments.Contracts.DTO.Appointment;

public class AppointmentUpdateDTO
{
    public Guid DoctorId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public int TimeSlot { get; set; }

    public int ServiceId { get; set; }

    public string OfficeId { get; set; }

    public int SpecializationId { get; set; }
}
