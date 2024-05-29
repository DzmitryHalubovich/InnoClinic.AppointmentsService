using Appointments.API.Extentions;
using Appointments.Domain.Interfaces;
using Appointments.Infrastructure.Data;
using Appointments.Infrastructure.Repositories;
using Appointments.Services;
using Appointments.Services.Abstraction;
using Appointments.Services.Abstractions.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IAppointmentResultsRepository, AppointmentResultsRepository>();
builder.Services.AddScoped<IAppointmentResultsService, AppointmentResultsService>();
builder.Services.AddAutoMapper(typeof(MapperProfile));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddSingleton<AppointmentsDbContext>();
builder.Services.AddScoped<IAppointmentsService, AppointmentsService>();
builder.Services.AddScoped<IAppointmentsRepository, AppointmentsRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler(opt => { });

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
