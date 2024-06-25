using Appointments.Domain.Interfaces;
using Appointments.RabbitMQ.Interfaces;
using Appointments.Services.Abstractions.BackgroundJobs;
using InnoClinic.SharedModels.MQMessages.Appointments;

namespace Appointments.Services.BackgroundJobs;

public class AppointmentsNotificationJobService : IAppointmentsNotificationJobService
{
    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly IPublisherServiceRabbitMq _publisherService;

    public AppointmentsNotificationJobService(IAppointmentsRepository appointmentsRepository, 
        IPublisherServiceRabbitMq publisherService)
    {
        _appointmentsRepository = appointmentsRepository;
        _publisherService = publisherService;
    }

    public async Task SendMessageWithAllApprovedAppointmentsToNotificationServer()
    {
        var approvedAppointments = await _appointmentsRepository.GetAllApprovedForNotitficationAsync();

        if (!approvedAppointments.Any())
        {
            return;
        }

        _publisherService.PublishAppointmentApprovedMessage(approvedAppointments.Select(appointment => 
            new AppointmentApprovedMessage()
            {
                AppointmentId = appointment.Id,
                PatientEmail = appointment.PatientEmail,
                AppointmentDate = appointment.AppointmentDate
            }).ToList());

        await _appointmentsRepository.SetNotificationIsSentAsync(approvedAppointments);
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
