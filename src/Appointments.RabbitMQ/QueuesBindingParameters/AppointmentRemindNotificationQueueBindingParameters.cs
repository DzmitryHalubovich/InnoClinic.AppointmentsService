namespace Appointments.RabbitMQ.QueuesBindingParameters;

public record AppointmentRemindNotificationQueueBindingParameters(string ExchangeName, string QueueName, string RoutingKey) 
    : BaseBindingQueueParameters(ExchangeName, QueueName, RoutingKey);
