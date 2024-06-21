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
    private readonly AppointmentApprovedQueueBindingParameters _bindingParameters;

    public ProducerServiceRabbitMq(IRabbitMqConnection connection, AppointmentApprovedQueueBindingParameters bindingParameters)
    {
        _connection = connection;
        _bindingParameters = bindingParameters;
    }

    public void Publish(IEnumerable<AppointmentApprovedMessage> messages)
    {
        using var channel = _connection.Connection.CreateModel();

        channel.ExchangeDeclare(_bindingParameters.ExchangeName, ExchangeType.Direct);

        channel.QueueDeclare(queue: _bindingParameters.QueueName,
                             durable: false,
                             exclusive: false,
                             autoDelete: false);

        channel.QueueBind(queue: _bindingParameters.QueueName,
                          exchange: _bindingParameters.ExchangeName,
                          routingKey: _bindingParameters.RoutingKey,
                          arguments: null);

        foreach (var message in messages)
        {
            var messageJsonFormat = JsonConvert.SerializeObject(message);

            var messageByteFormat = Encoding.UTF8.GetBytes(messageJsonFormat);

            channel.BasicPublish(exchange: _bindingParameters.ExchangeName,
                                 routingKey: _bindingParameters.RoutingKey,
                                 basicProperties: null,
                                 body: messageByteFormat);
        }
    }
}
