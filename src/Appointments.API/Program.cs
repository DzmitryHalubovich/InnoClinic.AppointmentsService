using Appointments.API;
using Appointments.API.Extentions;
using Appointments.Infrastructure.Data;
using FluentMigrator.Runner;
using Hangfire;
using MassTransit;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.SetInMemorySagaRepositoryProvider();

    var assembly = typeof(Program).Assembly;

    x.AddConsumer<OfficeUpdatedConsumer>();

    x.AddSagaStateMachines(assembly);
    x.AddSagas(assembly);
    x.AddActivities(assembly);

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("office-updated-profiles", queueConfigurator =>
        {
            queueConfigurator.Consumer<OfficeUpdatedConsumer>(context);
        });
    });
});

builder.ConfigureServices();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseBackgroundAppointmentApprovedNotificationJob();
app.UseHangfireDashboard();
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
//.RequireAuthorization("ApiScope");

app.MigrateDatabase();

app.Run();
