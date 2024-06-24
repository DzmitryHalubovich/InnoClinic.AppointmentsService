using Appointments.Services.Abstractions.BackgroundJobs;
using Hangfire;

namespace Appointments.API.Extentions;

public static class BackgroundJobExtensions
{
    public static IApplicationBuilder UseBackgroundJobs(this WebApplication app)
    {
        app.Services.GetRequiredService<IRecurringJobManager>()
            .AddOrUpdate<IAppointmentsNotificationJobService>(
                "send-message-to-notification-service",
                job => job.SendMessageWithAllApprovedAppointmentsToNotificationServer(),
                app.Configuration["BackgroundJobs:Outbox:Schedule"]);

        return app;
    }
}
