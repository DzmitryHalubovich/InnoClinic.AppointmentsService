namespace Appointments.RabbitMQ.QueuesBindingParameters;

public record ServiceDeletedBindingQueueParameters(string ExchangeName, string QueueName, string RoutingKey) 
    : BaseBindingQueueParameters(ExchangeName, QueueName, RoutingKey);
