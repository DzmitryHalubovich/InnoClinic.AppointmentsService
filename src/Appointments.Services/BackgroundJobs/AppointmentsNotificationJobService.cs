using Appointments.Domain.Interfaces;
using Appointments.Services.Abstractions.BackgroundJobs;
using InnoClinic.SharedModels.MQMessages.Appointments;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Appointments.Services.BackgroundJobs;

public class AppointmentsNotificationJobService : IAppointmentsNotificationJobService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly IPublishEndpoint _messageProducer;

    public AppointmentsNotificationJobService(IServiceProvider serviceProvider, 
        IAppointmentsRepository appointmentsRepository,
        IPublishEndpoint messageProducer)
    {
        _serviceProvider = serviceProvider;
        _appointmentsRepository = appointmentsRepository;
        _messageProducer = messageProducer;
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

            foreach (var approvedAppointment in approvedAppointmentsList)
            {
                await _messageProducer.Publish<AppointmentApprovedMessage>(new ()
                {
                    AppointmentId = approvedAppointment.Id,
                    PatientEmail = approvedAppointment.PatientEmail,
                    AppointmentDate = approvedAppointment.AppointmentDate
                });
            }

            await _appointmentsRepository.SetNotificationIsSentAsync(approvedAppointmentsList);
        }
    }

    public async Task SendNotificationAboutAppointment(Guid id)
    {
        var appointment = await _appointmentsRepository.GetByIdAsync(id);

        await _messageProducer.Publish<AppointmentRemindNotificationMessage>(new()
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
