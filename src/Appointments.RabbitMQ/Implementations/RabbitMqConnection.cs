using Appointments.RabbitMQ.Interfaces;
using RabbitMQ.Client;

namespace Appointments.RabbitMQ.Implementations;

public class RabbitMqConnection : IRabbitMqConnection, IDisposable
{
    private IConnection _connection;

    public IConnection Connection => _connection!;

    public RabbitMqConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
        };

        _connection = factory.CreateConnection();
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}
