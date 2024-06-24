namespace Appointments.Services.Abstraction;

public class AppointmentNotificationMessage
{
    public string PatientEmail { get; set; }

    public string PatientFullName { get; set; }

    public string DoctorFullName { get; set; }

    public string Date {  get; set; }

    public string Time { get; set; }

    public string ServiceName { get; set; }
}
