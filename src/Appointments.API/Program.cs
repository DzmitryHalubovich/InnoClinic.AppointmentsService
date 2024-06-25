using Appointments.API.Extentions;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureScopes();

builder.ConfigureDIContainers();

var app = builder.Build();

app.UseBackgroundJobs();

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
app.Run();
