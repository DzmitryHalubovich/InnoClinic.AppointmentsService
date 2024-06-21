using Appointments.Domain.Interfaces;
using Appointments.Infrastructure.Data;
using Appointments.Infrastructure.Repositories;
using Appointments.RabbitMQ.Implementations;
using Appointments.RabbitMQ.Interfaces;
using Appointments.RabbitMQ.QueuesBindingParameters;
using Appointments.Services.Abstraction;
using Appointments.Services.Abstractions.BackgroundJobs;
using Appointments.Services.Abstractions.Services;
using Appointments.Services.BackgroundJobs;
using Appointments.Services.Services;
using Hangfire;

namespace Appointments.API.Extentions;

public static class ConfigureServices
{
    public static void ConfigureScopes(this WebApplicationBuilder builder)
    {
        var appointmentApprovedBindingParameters = builder.Configuration
            .GetSection("RabbitMqProducerQueuesParameters:AppointmentApprovedEvent")
            .Get<AppointmentApprovedQueueBindingParameters>();

        var serviceDeletedBindingParameters = builder.Configuration
            .GetSection("RabbitMqProducerQueuesParameters:ServiceDeletedEvent")
            .Get<ServiceDeletedBindingQueueParameters>();

        var serviceChangedToInactive = builder.Configuration
            .GetSection("RabbitMqProducerQueuesParameters:ServiceStatusSetInactiveEnent")
            .Get<ServiceStatusSetInactiveBindingQueueParameters>();

        builder.Services.AddSingleton(appointmentApprovedBindingParameters!);
        builder.Services.AddSingleton(serviceDeletedBindingParameters!);
        builder.Services.AddSingleton(serviceChangedToInactive);

        builder.Services.AddSingleton<IRabbitMqConnection>(new RabbitMqConnection());
        builder.Services.AddSingleton<AppointmentsDbContext>();

        builder.Services.AddScoped<IPublisherServiceRabbitMq, ProducerServiceRabbitMq>();
        builder.Services.AddScoped<ISendMessageWithApprovedAppointmentsJob, SendMessageWithApprovedAppointmentsJob>();
        builder.Services.AddScoped<IAppointmentResultsRepository, AppointmentResultsRepository>();
        builder.Services.AddScoped<IAppointmentResultsService, AppointmentResultsService>();
        
        builder.Services.AddTransient<IAppointmentsService, AppointmentsService>();
        builder.Services.AddTransient<IAppointmentsRepository, AppointmentsRepository>();
    }

    public static void ConfigureDIContainers(this WebApplicationBuilder builder)
    {
        var bindingParameters = builder.Configuration
            .GetSection("RabbitMqProducerQueuesParameters:AppointmentApprovedEvent")
            .Get<BaseBindingQueueParameters>();

        builder.Services.AddHangfire(configuration =>
           configuration.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireSQLConnection")));
        builder.Services.AddHangfireServer(options => options.SchedulePollingInterval = TimeSpan.FromSeconds(1));


        builder.Services.AddHostedService<ConsumerServiceRabbitMq>();

        builder.Services.AddAutoMapper(typeof(MapperProfile));
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHttpClient<DocumentsRepository>();
    }
}
