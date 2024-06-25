namespace Appointments.RabbitMQ.QueuesBindingParameters;

public record AppointmentApprovedQueueBindingParameters(string ExchangeName, string QueueName, string RoutingKey) 
    : BaseBindingQueueParameters(ExchangeName, QueueName, RoutingKey);

