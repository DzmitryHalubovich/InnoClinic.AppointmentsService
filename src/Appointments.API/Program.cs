using Appointments.API.Extentions;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();

var app = builder.Build();

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

app.Run();
