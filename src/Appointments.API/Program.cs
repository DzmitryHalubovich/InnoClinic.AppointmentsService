using Appointments.API.Extentions;
using Appointments.Domain.Interfaces;
using Appointments.Infrastructure.Data;
using Appointments.Infrastructure.Repositories;
using Appointments.Presentation.RabbitMQ;
using Appointments.Services.Abstraction;
using Appointments.Services.Abstractions.BackgroundJobs;
using Appointments.Services.Abstractions.Services;
using Appointments.Services.BackgroundJobs;
using Appointments.Services.Services;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<RabbitListener>();

builder.Services.AddHangfire(configuration =>
    configuration.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireSQLConnection")));

builder.Services.AddScoped<ISendMessageWithApprovedAppointmentsJob, SendMessageWithApprovedAppointmentsJob>();
builder.Services.AddScoped<IAppointmentResultsRepository, AppointmentResultsRepository>();
builder.Services.AddScoped<IAppointmentResultsService, AppointmentResultsService>();
builder.Services.AddAutoMapper(typeof(MapperProfile));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddSingleton<AppointmentsDbContext>();
builder.Services.AddTransient<IAppointmentsService, AppointmentsService>();
builder.Services.AddTransient<IAppointmentsRepository, AppointmentsRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<DocumentsRepository>();

builder.Services.AddHangfireServer(options => options.SchedulePollingInterval = TimeSpan.FromSeconds(1));

var app = builder.Build();

app.UseBackgroundJobs();

app.UseHangfireDashboard();

//app.UseRabbitListener();

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
