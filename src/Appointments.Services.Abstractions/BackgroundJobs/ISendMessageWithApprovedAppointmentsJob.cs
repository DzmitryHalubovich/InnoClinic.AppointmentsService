namespace Appointments.Services.Abstractions.BackgroundJobs;

public interface ISendMessageWithApprovedAppointmentsJob
{
    public Task SendMessageWithAllApprovedAppointmentsToNotificationServer();
}
