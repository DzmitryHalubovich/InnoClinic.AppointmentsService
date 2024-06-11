using Appointments.Presentation.RabbitMQ;

namespace Appointments.API.Extentions;

public static class ApplicationBuilderExtentions
{
    private static RabbitListener _listener { get; set; }

    public static IApplicationBuilder UseRabbitListener(this  IApplicationBuilder app)
    {
        _listener = app.ApplicationServices.GetService<RabbitListener>();

        var lifeTime = app.ApplicationServices.GetService<Microsoft.AspNetCore.Hosting.IApplicationLifetime>();

        lifeTime.ApplicationStarted.Register(OnStarted);

        lifeTime.ApplicationStopping.Register(OnStopping);

        return app;
    }

    private static void OnStarted()
    {
        _listener.Register();
    }

    private static void OnStopping()
    {
        _listener.Deregister();
    }
}
