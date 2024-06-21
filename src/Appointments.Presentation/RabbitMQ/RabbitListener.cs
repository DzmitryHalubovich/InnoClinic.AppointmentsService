using Appointments.Services.Abstraction;
using Appointments.Services.Abstractions.RabbitMQ;
using InnoClinic.SharedModels.MQMessages.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Appointments.Presentation.RabbitMQ;

public class RabbitListener
{
    private readonly IAppointmentsService _appointmentsService;
    //private ConnectionFactory _connectionFactory;
    //private IConnection _connection;
    private readonly IRabbitMqConnection _rabbitMqConnection;
    private IModel _channel;

    public RabbitListener(IAppointmentsService appointmentsService, IRabbitMqConnection rabbitMqConnection)
    {
        _appointmentsService = appointmentsService;
        //_connectionFactory = new ConnectionFactory { HostName = "localhost" };
        //_connection = _connectionFactory.CreateConnection();
        _rabbitMqConnection = rabbitMqConnection;
        _channel = _rabbitMqConnection.Connection.CreateModel();
    }

    public void Register()
    {
        RegisterServiceDeletedQueue();
        RegisterServiceStatusChangedToInactive();
    }

    public void Deregister()
    {
        _channel.Close();
        //_connection.Close();
        _rabbitMqConnection.Connection.Close();
    }

    private void RegisterServiceDeletedQueue()
    {
        var queueName = "service_deleted_queue";
        var exchangeName = "service_deleted";

        _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

        _channel.QueueDeclare(queue: queueName,
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

        _channel.QueueBind(queue: queueName,
                           exchange: exchangeName,
                           routingKey: queueName);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();

            var deletedServiceInfo = JsonSerializer.Deserialize<ServiceDeletedMessage>(body);

            await _appointmentsService.DeleteEveryAppointmentForDeletedServiceAsync(deletedServiceInfo.ServiceId);

            Console.WriteLine($" [x] Deleted :'{deletedServiceInfo.ServiceId}'");

            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
    }

    private void RegisterServiceStatusChangedToInactive()
    {
        var queueName = "service_status_changed_to_inactive_queue";
        var exchangeName = "service_status_changed_to_inactive";

        _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

        _channel.QueueDeclare(queue: queueName,
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

        _channel.QueueBind(queue: queueName,
                           exchange: exchangeName,
                           routingKey: queueName);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();

            var changedServiceInfo = JsonSerializer.Deserialize<ServiceStatusChangedToInactiveMessage>(body);

            await _appointmentsService.DeleteEveryAppointmentForDeletedServiceAsync(changedServiceInfo.ServiceId);

            Console.WriteLine($" [x] Status changed :'{changedServiceInfo.ServiceId}'");
        };

        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }
}
