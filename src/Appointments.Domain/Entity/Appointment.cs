using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Appointments.Domain.Entity
{
    public class Appointment
    {
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        [EmailAddress]
        public string PatientEmail { get; set; } = null!;

        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        public string OfficeId { get; set; } = null!;

        [Required]
        public int SpecializationId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public int TimeSlot { get; set; }

        [DefaultValue(false)]
        public bool IsApproved { get; set; }

        [DefaultValue(false)]
        public bool NotificationIsSent { get; set; }
    }
}
