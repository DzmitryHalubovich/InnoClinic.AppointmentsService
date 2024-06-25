namespace Appointments.RabbitMQ.QueuesBindingParameters;

public record AppointmentResultUpdatedQueueBindingParameters(string ExchangeName, string QueueName, string RoutingKey) 
    : BaseBindingQueueParameters(ExchangeName, QueueName, RoutingKey);
