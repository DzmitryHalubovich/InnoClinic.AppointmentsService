using Appointments.Contracts;
using Appointments.Domain.Interfaces;
using Appointments.Services.Abstractions.BackgroundJobs;

namespace Appointments.Services.BackgroundJobs;

public class SendMessageWithApprovedAppointmentsJob : ISendMessageWithApprovedAppointmentsJob
{
    private readonly IAppointmentsRepository _appointmentsRepository;

    public SendMessageWithApprovedAppointmentsJob(IAppointmentsRepository appointmentsRepository)
    {
        _appointmentsRepository = appointmentsRepository;
    }

    public async void SendMessageWithAllApprovedAppointmentsToNotificationServer()
    {
        var approvedAppointments = await _appointmentsRepository.GetAllApprovedForNotitfication();

        Console.WriteLine("Elements count: " + approvedAppointments.Count());

        if (!approvedAppointments.Any())
        {
            return;
        }

        


    }
}
