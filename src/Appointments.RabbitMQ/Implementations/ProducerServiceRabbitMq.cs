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
    private readonly AppointmentRemindNotificationQueueBindingParameters _bindingAppointmentRemindNotificationParameters;
    private readonly AppointmentResultCreatedQueueBindingParameters _bindingAppointmentResultCreatedParameters;
    private readonly AppointmentResultUpdatedQueueBindingParameters _bindingAppointmentResultUpdatedParameters;

    public ProducerServiceRabbitMq(IRabbitMqConnection connection, 
        AppointmentApprovedQueueBindingParameters bindingAppointmentApprovedParameters,
        AppointmentRemindNotificationQueueBindingParameters bindingApplointmentRemindNotificationParameters,
        AppointmentResultCreatedQueueBindingParameters bindingAppointmentResultCreatedParameters,
        AppointmentResultUpdatedQueueBindingParameters bindingAppointmentResultUpdatedParameters)
    {
        _connection = connection;
        _bindingAppointmentApprovedParameters = bindingAppointmentApprovedParameters;
        _bindingAppointmentRemindNotificationParameters = bindingApplointmentRemindNotificationParameters;
        _bindingAppointmentResultCreatedParameters = bindingAppointmentResultCreatedParameters;
        _bindingAppointmentResultUpdatedParameters = bindingAppointmentResultUpdatedParameters;
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

    public void PublishAppointmentResultCreatedMessage(AppointmentResultCreatedMessage message)
    {
        using var channel = _connection.Connection.CreateModel();

        SetUpQueue(_bindingAppointmentResultCreatedParameters, channel);

        var messageJsonFormat = JsonConvert.SerializeObject(message);

        var messageByteFormat = Encoding.UTF8.GetBytes(messageJsonFormat);

        PublishMessage(_bindingAppointmentResultCreatedParameters, channel, messageByteFormat);
    }

    public void PublishAppointmentResultUpdatedMessage(AppointmentResultUpdatedMessage message)
    {
        using var channel = _connection.Connection.CreateModel();

        SetUpQueue(_bindingAppointmentResultUpdatedParameters, channel);

        var messageJsonFormat = JsonConvert.SerializeObject(message);

        var messageByteFormat = Encoding.UTF8.GetBytes(messageJsonFormat);

        PublishMessage(_bindingAppointmentResultUpdatedParameters, channel, messageByteFormat);
    }

    public void PublishRemindNotification (AppointmentRemindNotificationMessage message)
    {
        using var channel = _connection.Connection.CreateModel();

        SetUpQueue(_bindingAppointmentRemindNotificationParameters, channel);

        var messageJsonFormat = JsonConvert.SerializeObject(message);

        var messageByteFormat = Encoding.UTF8.GetBytes(messageJsonFormat);

        PublishMessage(_bindingAppointmentRemindNotificationParameters, channel, messageByteFormat);
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
