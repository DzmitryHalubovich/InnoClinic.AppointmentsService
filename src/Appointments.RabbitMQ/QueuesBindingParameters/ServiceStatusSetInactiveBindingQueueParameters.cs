namespace Appointments.RabbitMQ.QueuesBindingParameters;

public record ServiceStatusSetInactiveBindingQueueParameters(string ExchangeName, string QueueName, string RoutingKey) 
    : BaseBindingQueueParameters(ExchangeName, QueueName, RoutingKey);
