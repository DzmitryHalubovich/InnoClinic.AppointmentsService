namespace Appointments.RabbitMQ.QueuesBindingParameters;

public record AppointmentApprovedQueueBindingParameters : BaseBindingQueueParameters
{
    public AppointmentApprovedQueueBindingParameters(string ExchangeName, string QueueName, string RoutingKey)
        : base(ExchangeName, QueueName, RoutingKey)
    { }
}
