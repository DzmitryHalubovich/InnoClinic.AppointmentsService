using Appointments.Services.Abstractions.RabbitMQ;
using RabbitMQ.Client;

namespace Appointments.Services.RabbitMQ;

public class RabbitMqConnection : IRabbitMqConnection, IDisposable
{
    private IConnection? _connection;

    public IConnection Connection => _connection!;

    public RabbitMqConnection()
    {
        InitializeConnection();
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }

    private void InitializeConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        _connection = factory.CreateConnection();

        EnsureQueuesExist(_connection);
    }

    private void EnsureQueuesExist(IConnection connection)
    {
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "appointment_approved_queue",
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);


    }
}
