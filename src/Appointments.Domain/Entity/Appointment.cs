using System.ComponentModel.DataAnnotations;

namespace Appointments.Domain.Entity
{
    public class Appointment
    {
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public TimeSpan TimeSlot { get; set; }

        public bool IsApproved { get; set; }
    }
}
