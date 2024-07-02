using Appointments.RabbitMQ.Interfaces;
using Appointments.RabbitMQ.QueuesBindingParameters;
using Appointments.Services.Abstraction;
using InnoClinic.SharedModels.MQMessages.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Appointments.RabbitMQ.Implementations;

public class ConsumerServiceRabbitMq : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ServiceDeletedBindingQueueParameters _serviceDeletedQueueBindingParameters;
    private readonly ServiceStatusSetInactiveBindingQueueParameters _serviceStatusSetInactiveBindingParameters;
    private readonly IRabbitMqConnection _connection;
    private IModel _channel;

    public ConsumerServiceRabbitMq(IRabbitMqConnection connection, IServiceProvider serviceProvider,
        ServiceDeletedBindingQueueParameters serviceDeletedQueueBindingParameters, 
        ServiceStatusSetInactiveBindingQueueParameters serviceStatusSetInactiveBindingParameters)
    {
        _connection = connection;
        _serviceProvider = serviceProvider;
        _channel = _connection.Connection.CreateModel();
        _serviceDeletedQueueBindingParameters = serviceDeletedQueueBindingParameters;
        _serviceStatusSetInactiveBindingParameters = serviceStatusSetInactiveBindingParameters;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (IServiceScope scope = _serviceProvider.CreateScope())
        {
            var appointmentService = scope.ServiceProvider.GetService<IAppointmentsService>();

            RegisterServiceDeletedEventConsumer(appointmentService);
            RegisterServiceStatusChangedToIncativeEventConsumer(appointmentService);

            return Task.CompletedTask;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Close();

        return Task.CompletedTask;
    }

    private void RegisterServiceDeletedEventConsumer(IAppointmentsService appoitnmentService)
    {
        SetQueue(_serviceDeletedQueueBindingParameters);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (sender, args) =>
        {
            var body = args.Body.ToArray();

            var message = JsonSerializer.Deserialize<ServiceDeletedMessage>(body);

            await appoitnmentService.DeleteEveryAppointmentForDeletedServiceAsync(message.ServiceId);

            _channel.BasicAck(args.DeliveryTag, false);
        };

        _channel.BasicConsume(queue: _serviceDeletedQueueBindingParameters.QueueName, autoAck: false, consumer: consumer);
    }

    private void RegisterServiceStatusChangedToIncativeEventConsumer(IAppointmentsService appoitnmentService) 
    {
        SetQueue(_serviceStatusSetInactiveBindingParameters);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (sender, args) =>
        {
            var body = args.Body.ToArray();

            var message = JsonSerializer.Deserialize<ServiceStatusChangedToInactiveMessage>(body);

            await appoitnmentService.DeleteEveryAppointmentForDeletedServiceAsync(message.ServiceId);

            _channel.BasicAck(args.DeliveryTag, false);
        };

        _channel.BasicConsume(queue: _serviceStatusSetInactiveBindingParameters.QueueName, autoAck: false, consumer: consumer);
    }

    private void SetQueue(BaseBindingQueueParameters bindingQueueParameters)
    {
        _channel.ExchangeDeclare(exchange: bindingQueueParameters.ExchangeName, type: ExchangeType.Direct);

        _channel.QueueDeclare(queue: bindingQueueParameters.QueueName,
                              durable: false,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

        _channel.QueueBind(queue: bindingQueueParameters.QueueName,
                           exchange: bindingQueueParameters.ExchangeName,
                           routingKey: bindingQueueParameters.RoutingKey,
                           arguments: null);
    }
}
