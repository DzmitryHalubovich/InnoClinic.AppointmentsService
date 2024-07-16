using Appointments.RabbitMQ.Interfaces;
using Appointments.RabbitMQ.QueuesBindingParameters;
using InnoClinic.SharedModels.MQMessages.Appointments;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Appointments.RabbitMQ.Implementations;

public class ProducerServiceRabbitMq : IPublisherServiceRabbitMq
{
    private readonly IRabbitMqConnection _connection;
    private readonly AppointmentApprovedQueueBindingParameters _bindingAppointmentApprovedParameters;

    public ProducerServiceRabbitMq(IRabbitMqConnection connection, 
        AppointmentApprovedQueueBindingParameters bindingAppointmentApprovedParameters)
    {
        _connection = connection;
        _bindingAppointmentApprovedParameters = bindingAppointmentApprovedParameters;
    }

    public void PublishMessage<T>(BaseBindingQueueParameters queueParameters, T message)
    {
        using var channel = _connection.Connection.CreateModel();

        SetUpQueue(queueParameters, channel);

        var messageJsonFormat = JsonConvert.SerializeObject(message);

        var messageByteFormat = Encoding.UTF8.GetBytes(messageJsonFormat);

        PublishMessage(queueParameters, channel, messageByteFormat);
    }

    public void PublishAppointmentApprovedMessage(IEnumerable<AppointmentApprovedMessage> messages)
    {
        using var channel = _connection.Connection.CreateModel();

        SetUpQueue(_bindingAppointmentApprovedParameters, channel);

        foreach (var message in messages)
        {
            var messageJsonFormat = JsonConvert.SerializeObject(message);

            var messageByteFormat = Encoding.UTF8.GetBytes(messageJsonFormat);

            PublishMessage(_bindingAppointmentApprovedParameters, channel, messageByteFormat);
        }
    }

    private void SetUpQueue(BaseBindingQueueParameters parameters, IModel channel)
    {
        channel.ExchangeDeclare(parameters.ExchangeName, ExchangeType.Direct);

        channel.QueueDeclare(queue: parameters.QueueName,
                             durable: false,
                             exclusive: false,
                             autoDelete: false);

        channel.QueueBind(queue: parameters.QueueName,
                          exchange: parameters.ExchangeName,
                          routingKey: parameters.RoutingKey,
                          arguments: null);
    }

    private void PublishMessage(BaseBindingQueueParameters parameters, IModel channel, byte[] message)
    {
        channel.BasicPublish(exchange: parameters.ExchangeName,
                             routingKey: parameters.RoutingKey,
                             basicProperties: null,
                             body: message);
    }
}
