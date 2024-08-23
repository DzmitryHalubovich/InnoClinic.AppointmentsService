using Appointments.Domain.Interfaces;
using InnoClinic.SharedModels.MQMessages.Services;
using MassTransit;

namespace Appointments.Infrastructure.MassTransit;

public class ServiceDeletedConsumer : IConsumer<ServiceDeletedMessage>
{
    private readonly IAppointmentsRepository _appointmentsRepository;

    public ServiceDeletedConsumer(IAppointmentsRepository appointmentsRepository)
    {
        _appointmentsRepository = appointmentsRepository;
    }

    public async Task Consume(ConsumeContext<ServiceDeletedMessage> context)
    {
        await _appointmentsRepository.DeleteAllForDeletedServiceAsync(context.Message.ServiceId);
    }
}
