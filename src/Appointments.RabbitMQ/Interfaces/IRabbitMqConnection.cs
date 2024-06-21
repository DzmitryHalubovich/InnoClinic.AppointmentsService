using RabbitMQ.Client;

namespace Appointments.RabbitMQ.Interfaces;

public interface IRabbitMqConnection
{
    IConnection Connection { get; }
}
