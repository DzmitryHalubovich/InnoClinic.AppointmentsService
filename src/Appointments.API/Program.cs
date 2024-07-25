using Appointments.API.Extentions;
using Appointments.Infrastructure.Data;
using FluentMigrator.Runner;
using Hangfire;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

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
