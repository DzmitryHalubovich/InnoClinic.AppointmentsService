namespace Appointments.Services.Abstractions.BackgroundJobs;

public interface IAppointmentsNotificationJobService
{
    public Task SendMessageWithAllApprovedAppointmentsToNotificationServer();

    public Task SendNotificationAboutAppointment(Guid id);
}
