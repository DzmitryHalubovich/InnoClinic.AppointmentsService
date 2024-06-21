using Appointments.Domain.Interfaces;
using Appointments.RabbitMQ.Interfaces;
using Appointments.Services.Abstractions.BackgroundJobs;
using InnoClinic.SharedModels.MQMessages.Appointments;

namespace Appointments.Services.BackgroundJobs;

public class SendMessageWithApprovedAppointmentsJob : ISendMessageWithApprovedAppointmentsJob
{
    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly IPublisherServiceRabbitMq _publisherService;

    public SendMessageWithApprovedAppointmentsJob(IAppointmentsRepository appointmentsRepository, 
        IPublisherServiceRabbitMq publisherService)
    {
        _appointmentsRepository = appointmentsRepository;
        _publisherService = publisherService;
    }

    public async Task SendMessageWithAllApprovedAppointmentsToNotificationServer()
    {
        var approvedAppointments = await _appointmentsRepository.GetAllApprovedForNotitficationAsync();

        Console.WriteLine("Elements count: " + approvedAppointments.Count());

        if (!approvedAppointments.Any())
        {
            return;
        }

        _publisherService.Publish(approvedAppointments.Select(appointment => 
            new AppointmentApprovedMessage()
            {
                AppointmentId = appointment.Id,
                PatientEmail = appointment.PatientEmail,
                AppointmentDate = appointment.AppointmentDate
            }).ToList());

        await _appointmentsRepository.SetNotificationIsSentAsync(approvedAppointments);
    }
}
