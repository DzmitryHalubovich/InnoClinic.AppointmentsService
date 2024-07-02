using Appointments.Services.Abstractions.BackgroundJobs;
using Hangfire;

namespace Appointments.API.Extentions;

public static class BackgroundJobExtensions
{
    public static IApplicationBuilder UseBackgroundAppointmentApprovedNotificationJob(this WebApplication app)
    {
        app.Services.GetRequiredService<IRecurringJobManager>()
            .AddOrUpdate<IAppointmentsNotificationJobService>(
                "send-message-to-notification-service",
                job => job.SendMessageWithAllApprovedAppointmentsToNotificationServer(),
                app.Configuration["BackgroundJobs:GatherAllApprovedAppointments:Schedule"]);

        return app;
    }
}
