using Appointments.Services.Abstractions.BackgroundJobs;
using Hangfire;

namespace Appointments.API.Extentions;

public static class BackgroundJobExtensions
{
    public static IApplicationBuilder UseBackgroundAppointmentApprovedNotificationJob(this WebApplication app)
    {
        app.Services.GetRequiredService<IRecurringJobManager>()
            .AddOrUpdate<IAppointmentsNotificationJobService>(
                app.Configuration["BackgroundJobs:GatherAllApprovedAppointments:RequrringJobId"],
                job => job.SendMessageWithAllApprovedAppointmentsToNotificationServer(),
                app.Configuration["BackgroundJobs:GatherAllApprovedAppointments:Schedule"]);

        return app;
    }
}
