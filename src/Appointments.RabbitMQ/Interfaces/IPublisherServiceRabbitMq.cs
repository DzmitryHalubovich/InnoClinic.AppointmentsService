using Appointments.RabbitMQ.QueuesBindingParameters;
using InnoClinic.SharedModels.MQMessages.Appointments;

namespace Appointments.RabbitMQ.Interfaces;

public interface IPublisherServiceRabbitMq
{
    public void PublishAppointmentApprovedMessage(IEnumerable<AppointmentApprovedMessage> message);

    public void PublishRemindNotification(AppointmentRemindNotificationMessage message);

    public void PublishAppointmentResultCreatedMessage(AppointmentResultCreatedMessage message);

    public void PublishAppointmentResultUpdatedMessage(AppointmentResultUpdatedMessage message);
}
