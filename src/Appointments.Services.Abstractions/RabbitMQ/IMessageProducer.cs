using InnoClinic.SharedModels.MQMessages.Appointments;

namespace Appointments.Services.Abstractions.RabbitMQ;

public interface IMessageProducer 
{
    void SendNotificateUsersMessage(IEnumerable<AppointmentApprovedMessage> messages);
}
