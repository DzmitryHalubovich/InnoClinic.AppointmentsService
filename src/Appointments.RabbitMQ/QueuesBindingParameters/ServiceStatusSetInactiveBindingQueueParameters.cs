namespace Appointments.RabbitMQ.QueuesBindingParameters;

public record ServiceStatusSetInactiveBindingQueueParameters : BaseBindingQueueParameters
{
    public ServiceStatusSetInactiveBindingQueueParameters(string ExchangeName, string QueueName, string RoutingKey) : 
        base(ExchangeName, QueueName, RoutingKey)
    {
    }
}
