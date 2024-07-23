using Appointments.RabbitMQ.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Appointments.RabbitMQ.Implementations;

public class RabbitMqConnection : IRabbitMqConnection, IDisposable
{
    private IConnection _connection;

    public IConnection Connection => _connection!;

    public RabbitMqConnection(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration.GetSection("RabbitMQ:HostName").Value,
        };

        _connection = factory.CreateConnection();
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}
