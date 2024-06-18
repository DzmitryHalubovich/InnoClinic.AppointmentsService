using Appointments.Domain.Interfaces;
using Appointments.Services.Abstractions.BackgroundJobs;
using Appointments.Services.Abstractions.RabbitMQ;

namespace Appointments.Services.BackgroundJobs;

public class SendMessageWithApprovedAppointmentsJob : ISendMessageWithApprovedAppointmentsJob
{
    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly IMessageProducer _messageProducer;

    public SendMessageWithApprovedAppointmentsJob(IAppointmentsRepository appointmentsRepository, 
        IMessageProducer messageProducer)
    {
        _appointmentsRepository = appointmentsRepository;
        _messageProducer = messageProducer;
    }

    public async void SendMessageWithAllApprovedAppointmentsToNotificationServer()
    {
        var approvedAppointments = await _appointmentsRepository.GetAllApprovedForNotitfication();

        Console.WriteLine("Elements count: " + approvedAppointments.Count());

        if (!approvedAppointments.Any())
        {
            return;
        }

        _messageProducer.SendNotificateUsersMessage(approvedAppointments.Select(appointment => 
            new AppointmentApprovedMessage()
            {
                AppointmentId = appointment.Id,
                PatientEmail = appointment.PatientEmail,
                AppointmentDate = appointment.AppointmentDate
            }).ToList());

        await _appointmentsRepository.SetNotificationIsSent(approvedAppointments);
    }
}
