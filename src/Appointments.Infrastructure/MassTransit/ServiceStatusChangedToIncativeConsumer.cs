using Appointments.Domain.Interfaces;
using InnoClinic.SharedModels.MQMessages.Services;
using MassTransit;

namespace Appointments.Infrastructure.MassTransit;

public class ServiceStatusChangedToIncativeConsumer : IConsumer<ServiceStatusChangedToInactiveMessage>
{
    private readonly IAppointmentsRepository _appointmentsRepository;

    public ServiceStatusChangedToIncativeConsumer(IAppointmentsRepository appointmentsRepository)
    {
        _appointmentsRepository = appointmentsRepository;
    }

    public async Task Consume(ConsumeContext<ServiceStatusChangedToInactiveMessage> context)
    {
        await _appointmentsRepository.DeleteAllForDeletedServiceAsync(context.Message.ServiceId);
    }
}
