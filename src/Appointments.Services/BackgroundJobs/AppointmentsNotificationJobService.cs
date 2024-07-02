using Appointments.Domain.Interfaces;
using Appointments.RabbitMQ.Interfaces;
using Appointments.Services.Abstractions.BackgroundJobs;
using InnoClinic.SharedModels.MQMessages.Appointments;
using Microsoft.Extensions.DependencyInjection;

namespace Appointments.Services.BackgroundJobs;

public class AppointmentsNotificationJobService : IAppointmentsNotificationJobService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly IPublisherServiceRabbitMq _publisherService;

    public AppointmentsNotificationJobService(IServiceProvider serviceProvider, IAppointmentsRepository appointmentsRepository, 
        IPublisherServiceRabbitMq publisherService)
    {
        _serviceProvider = serviceProvider;
        _appointmentsRepository = appointmentsRepository;
        _publisherService = publisherService;
    }

    public async Task SendMessageWithAllApprovedAppointmentsToNotificationServer()
    {
        using (IServiceScope scope = _serviceProvider.CreateScope())
        {
            var scopedProvider = scope.ServiceProvider;

            var appointmentRepository = scope.ServiceProvider.GetRequiredService<IAppointmentsRepository>();

            var approvedAppointmentsList = await _appointmentsRepository.GetAllApprovedForNotitficationAsync();

            if (!approvedAppointmentsList.Any())
            {
                return;
            }

            var publisherService = scope.ServiceProvider.GetRequiredService<IPublisherServiceRabbitMq>();

            publisherService.PublishAppointmentApprovedMessage(approvedAppointmentsList.Select(appointment =>
                new AppointmentApprovedMessage()
                {
                    AppointmentId = appointment.Id,
                    PatientEmail = appointment.PatientEmail,
                    AppointmentDate = appointment.AppointmentDate
                }).ToList());

            await _appointmentsRepository.SetNotificationIsSentAsync(approvedAppointmentsList);
        }
    }

    public async Task SendNotificationAboutAppointment(Guid id)
    {
        var appointment = await _appointmentsRepository.GetByIdAsync(id);
        
        _publisherService.PublishRemindNotification(new AppointmentRemindNotificationMessage()
        {
            PatientEmail = appointment.PatientEmail,
            PatientFullName = appointment.PatientFullName,
            DoctorFullName = appointment.DoctorFullName,
            ServiceName = appointment.ServiceName,
            Date = appointment.AppointmentDate.ToShortDateString(),
            Time = appointment.AppointmentDate.ToShortTimeString()
        });
    }
}
