using InnoClinic.SharedModels.MQMessages.Appointments;

namespace Appointments.RabbitMQ.Interfaces;

public interface IPublisherServiceRabbitMq
{
    public void Publish(IEnumerable<AppointmentApprovedMessage> message);
}
