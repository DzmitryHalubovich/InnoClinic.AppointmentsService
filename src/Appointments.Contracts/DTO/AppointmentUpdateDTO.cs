namespace Appointments.Contracts.DTO;

public class AppointmentUpdateDTO
{
    public DateTime AppointmentDate { get; set; }

    public int TimeSlot { get; set; }

    public int ServiceId { get; set; }

    public Guid DoctorId { get; set; }
}
