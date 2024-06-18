using RabbitMQ.Client;

namespace Appointments.Services.Abstractions.RabbitMQ;

public interface IRabbitMqConnection
{
    IConnection Connection { get; }
}
