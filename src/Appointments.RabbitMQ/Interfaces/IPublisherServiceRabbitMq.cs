using Appointments.RabbitMQ.QueuesBindingParameters;
using InnoClinic.SharedModels.MQMessages.Appointments;

namespace Appointments.RabbitMQ.Interfaces;

public interface IPublisherServiceRabbitMq
{
    public void PublishMessage<T>(BaseBindingQueueParameters queueParameters, T message);

    public void PublishAppointmentApprovedMessage(IEnumerable<AppointmentApprovedMessage> message);
}
