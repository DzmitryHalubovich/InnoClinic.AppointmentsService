using Appointments.Services.Abstractions.RabbitMQ;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Appointments.Services.RabbitMQ;

public class MessageProducer : IMessageProducer
{
    private readonly IRabbitMqConnection _connection;

    public MessageProducer(IRabbitMqConnection connection)
    {
        _connection = connection;
    }

    public void SendNotificateUsersMessage(IEnumerable<AppointmentApprovedMessage> messages)
    {
        using var channel = _connection.Connection.CreateModel();

        var queueName = "appointment_approved_queue";
        var exchangeName = "appointment_approved";

        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

        channel.QueueBind(queue: queueName,
                          exchange: exchangeName,
                          routingKey: queueName);

        foreach (var message in messages)
        {
            var json = JsonConvert.SerializeObject(message);

            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: exchangeName,
                                 routingKey: queueName,
                                 basicProperties: null,
                                 body: body);
        }
    }
}
