namespace Appointments.RabbitMQ.QueuesBindingParameters;

public record ServiceDeletedBindingQueueParameters : BaseBindingQueueParameters
{
    public ServiceDeletedBindingQueueParameters(string ExchangeName, string QueueName, string RoutingKey) 
        : base(ExchangeName, QueueName, RoutingKey)
    {
    }
}
