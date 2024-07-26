using Appointments.Contracts;
using Appointments.Domain.Interfaces;
using InnoClinic.SharedModels.MQMessages.Offices;
using MassTransit;

namespace Appointments.API;

public class OfficeUpdatedConsumer : IConsumer<OfficeUpdatedMessage>
{
    private readonly IAppointmentsRepository _appointmentsRepository;

    public OfficeUpdatedConsumer(IAppointmentsRepository appointmentsRepository)
    {
        _appointmentsRepository = appointmentsRepository;
    }

    public async Task Consume(ConsumeContext<OfficeUpdatedMessage> context)
    {
        var officeUpdatedMessage = context.Message;

        var offices = await _appointmentsRepository.GetAllAsync(new QueryParameters { OfficeId = officeUpdatedMessage .OfficeId });

        foreach (var office in offices)
        {
            office.OfficeAddress = officeUpdatedMessage.OfficeAddress;

            await _appointmentsRepository.UpdateAsync(office);
        }
    }
}
