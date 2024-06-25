using Appointments.RabbitMQ.Interfaces;
using Appointments.RabbitMQ.QueuesBindingParameters;
using Appointments.Services.Abstractions;
using InnoClinic.SharedModels.MQMessages.Appointments;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Appointments.RabbitMQ.Implementations;

public class ProducerServiceRabbitMq : IPublisherServiceRabbitMq
{
    private readonly IRabbitMqConnection _connection;
    private readonly AppointmentApprovedQueueBindingParameters _bindingAppointmentApprovedParameters;
    private readonly AppointmentNotificationQueueBindingParameters _bindingNotificationParameters;
    private readonly AppointmentResultCreatedQueueBindingParameters _bindingAppointmentResultCreatedParameters;
    private readonly AppointmentResultUpdatedQueueBindingParameters _bindingAppointmentResultUpdatedParameters;

    public ProducerServiceRabbitMq(IRabbitMqConnection connection, 
        AppointmentApprovedQueueBindingParameters bindingAppointmentApprovedParameters,
        AppointmentNotificationQueueBindingParameters bindingNotificationParameters,
        AppointmentResultCreatedQueueBindingParameters bindingAppointmentResultCreatedParameters,
        AppointmentResultUpdatedQueueBindingParameters bindingAppointmentResultUpdatedParameters)
    {
        _connection = connection;
        _bindingAppointmentApprovedParameters = bindingAppointmentApprovedParameters;
        _bindingNotificationParameters = bindingNotificationParameters;
        _bindingAppointmentResultCreatedParameters = bindingAppointmentResultCreatedParameters;
        _bindingAppointmentResultUpdatedParameters = bindingAppointmentResultUpdatedParameters;
    }

    public void PublishAppointmentApprovedMessage(IEnumerable<AppointmentApprovedMessage> messages)
    {
        using var channel = _connection.Connection.CreateModel();

        channel.ExchangeDeclare(_bindingAppointmentApprovedParameters.ExchangeName, ExchangeType.Direct);

        channel.QueueDeclare(queue: _bindingAppointmentApprovedParameters.QueueName,
                             durable: false,
                             exclusive: false,
                             autoDelete: false);

        channel.QueueBind(queue: _bindingAppointmentApprovedParameters.QueueName,
                          exchange: _bindingAppointmentApprovedParameters.ExchangeName,
                          routingKey: _bindingAppointmentApprovedParameters.RoutingKey,
                          arguments: null);

        foreach (var message in messages)
        {
            var messageJsonFormat = JsonConvert.SerializeObject(message);

            var messageByteFormat = Encoding.UTF8.GetBytes(messageJsonFormat);

            channel.BasicPublish(exchange: _bindingAppointmentApprovedParameters.ExchangeName,
                                 routingKey: _bindingAppointmentApprovedParameters.RoutingKey,
                                 basicProperties: null,
                                 body: messageByteFormat);
        }
    }

    public void PublishAppointmentResultCreatedMessage(AppointmentResultCreatedMessage message)
    {
        using var channel = _connection.Connection.CreateModel();

        channel.ExchangeDeclare(_bindingAppointmentResultCreatedParameters.ExchangeName, ExchangeType.Direct);

        channel.QueueDeclare(queue: _bindingAppointmentResultCreatedParameters.QueueName,
                             durable: false,
                             exclusive: false,
                             autoDelete: false);

        channel.QueueBind(queue: _bindingAppointmentResultCreatedParameters.QueueName,
                          exchange: _bindingAppointmentResultCreatedParameters.ExchangeName,
                          routingKey: _bindingAppointmentResultCreatedParameters.RoutingKey,
                          arguments: null);
        
        var messageJsonFormat = JsonConvert.SerializeObject(message);

        var messageByteFormat = Encoding.UTF8.GetBytes(messageJsonFormat);

        channel.BasicPublish(exchange: _bindingAppointmentResultCreatedParameters.ExchangeName,
                                routingKey: _bindingAppointmentResultCreatedParameters.RoutingKey,
                                basicProperties: null,
                                body: messageByteFormat);
    }

    public void PublishAppointmentResultUpdatedMessage(AppointmentResultUpdatedMessage message)
    {
        using var channel = _connection.Connection.CreateModel();

        channel.ExchangeDeclare(_bindingAppointmentResultUpdatedParameters.ExchangeName, ExchangeType.Direct);

        channel.QueueDeclare(queue: _bindingAppointmentResultUpdatedParameters.QueueName,
                             durable: false,
                             exclusive: false,
                             autoDelete: false);

        channel.QueueBind(queue: _bindingAppointmentResultUpdatedParameters.QueueName,
                          exchange: _bindingAppointmentResultUpdatedParameters.ExchangeName,
                          routingKey: _bindingAppointmentResultUpdatedParameters.RoutingKey,
                          arguments: null);

        var messageJsonFormat = JsonConvert.SerializeObject(message);

        var messageByteFormat = Encoding.UTF8.GetBytes(messageJsonFormat);

        channel.BasicPublish(exchange: _bindingAppointmentResultUpdatedParameters.ExchangeName,
                                routingKey: _bindingAppointmentResultUpdatedParameters.RoutingKey,
                                basicProperties: null,
                                body: messageByteFormat);
    }

    public void PublishNotification (AppointmentRemindNotificationMessage message)
    {
        using var channel = _connection.Connection.CreateModel();

        channel.ExchangeDeclare(_bindingNotificationParameters.ExchangeName, ExchangeType.Direct);

        channel.QueueDeclare(queue: _bindingNotificationParameters.QueueName,
                             durable: false,
                             exclusive: false,
                             autoDelete: false);

        channel.QueueBind(queue: _bindingNotificationParameters.QueueName,
                          exchange: _bindingNotificationParameters.ExchangeName,
                          routingKey: _bindingNotificationParameters.RoutingKey,
                          arguments: null);

        var messageJsonFormat = JsonConvert.SerializeObject(message);

        var messageByteFormat = Encoding.UTF8.GetBytes(messageJsonFormat);

        channel.BasicPublish(exchange: _bindingNotificationParameters.ExchangeName,
                                 routingKey: _bindingNotificationParameters.RoutingKey,
                                 basicProperties: null,
                                 body: messageByteFormat);
    }
}
