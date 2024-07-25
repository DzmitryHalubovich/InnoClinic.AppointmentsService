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
using FluentMigrator.Runner;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

namespace Appointments.API.Extentions;

public static class WebApplicationBuilderExtention
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((ctx, lc) =>
            lc.WriteTo.Console()
            .ReadFrom.Configuration(ctx.Configuration));

        builder.Services.AddLogging(c => c.AddFluentMigratorConsole())
            .AddFluentMigratorCore()
            .ConfigureRunner(c => c.AddPostgres11_0()
            .WithGlobalConnectionString(builder.Configuration.GetConnectionString("SQLConnection"))
            .ScanIn(Assembly.GetAssembly(typeof(InitialTables_202106280001))).For.Migrations());

        var appointmentApprovedBindingParameters = builder.Configuration
            .GetSection("RabbitMqProducerQueuesParameters:AppointmentApprovedEvent")
            .Get<AppointmentApprovedQueueBindingParameters>();

        var appointmentRemindNotificationBindingParameters = builder.Configuration
            .GetSection("RabbitMqProducerQueuesParameters:AppointmentNotificationEvent")
            .Get<AppointmentRemindNotificationQueueBindingParameters>();
        
        var appointmentResultUpdatedBindingParameters = builder.Configuration
            .GetSection("RabbitMqProducerQueuesParameters:AppointmentResultUpdatedEvent")
            .Get<AppointmentResultUpdatedQueueBindingParameters>();

        var appointmentResultCreatedBindingParameters = builder.Configuration
            .GetSection("RabbitMqProducerQueuesParameters:AppointmentResultCreatedEvent")
            .Get<AppointmentResultCreatedQueueBindingParameters>();

        var serviceDeletedBindingParameters = builder.Configuration
            .GetSection("RabbitMqProducerQueuesParameters:ServiceDeletedEvent")
            .Get<ServiceDeletedBindingQueueParameters>();

        var serviceChangedToInactive = builder.Configuration
            .GetSection("RabbitMqProducerQueuesParameters:ServiceStatusSetInactiveEnent")
            .Get<ServiceStatusSetInactiveBindingQueueParameters>();        

        builder.Services.AddSingleton(appointmentApprovedBindingParameters!);
        builder.Services.AddSingleton(appointmentRemindNotificationBindingParameters!);
        builder.Services.AddSingleton(appointmentResultCreatedBindingParameters!);
        builder.Services.AddSingleton(appointmentResultUpdatedBindingParameters!);
        builder.Services.AddSingleton(serviceDeletedBindingParameters!);
        builder.Services.AddSingleton(serviceChangedToInactive);

        builder.Services.AddSingleton<IRabbitMqConnection>(new RabbitMqConnection(builder.Configuration));
        builder.Services.AddSingleton<AppointmentsDbContext>();

        var migrationService = new AppointmentsDbContext(builder.Configuration);
        migrationService.EnsureDatabaseCreated(
            [
                builder.Configuration["AppointmentsDbName"],
                builder.Configuration["HangfireDbName"]
            ]);

        builder.Services.AddScoped<IAppointmentResultsRepository, AppointmentResultsRepository>();
        builder.Services.AddScoped<IAppointmentResultsService, AppointmentResultsService>();
        builder.Services.AddScoped<IAppointmentsRepository, AppointmentsRepository>();
        builder.Services.AddScoped<IPublisherServiceRabbitMq, ProducerServiceRabbitMq>();
        builder.Services.AddScoped<IAppointmentsNotificationJobService, AppointmentsNotificationJobService>();
        builder.Services.AddScoped<IAppointmentsService, AppointmentsService>();

        builder.Services.AddHttpClient<DocumentsServiceHttpClient>();

        builder.Services.AddAuthentication("Bearer")
           .AddJwtBearer("Bearer", options =>
           {
               options.Authority = "https://localhost:5005";

               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateAudience = false
               };
           });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "appointments.api");
            });
        });

        var bindingParameters = builder.Configuration
            .GetSection("RabbitMqProducerQueuesParameters:AppointmentApprovedEvent")
            .Get<BaseBindingQueueParameters>();

        builder.Services.AddHangfire(configuration =>
           configuration.UsePostgreSqlStorage(c => 
                c.UseNpgsqlConnection(builder.Configuration.GetConnectionString("HangfireSQLConnection"))));

        builder.Services.AddHangfireServer(options => options.SchedulePollingInterval = TimeSpan.FromSeconds(1));

        builder.Services.AddAutoMapper(typeof(MapperProfile));
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(opt =>
        {
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Place to add JWT with Bearer",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Name = "Bearer",
                    },
                    new List<string>()
                }
            });
        });

        builder.Services.AddHostedService<ConsumerServiceRabbitMq>();
    }
}
