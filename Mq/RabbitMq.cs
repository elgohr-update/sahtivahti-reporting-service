using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace ReportingService.Mq
{
    public class RabbitMq : IMq
    {
        private readonly ILogger<RabbitMq> _logger;
        private readonly MqConfig _config;
        private static IConnection _connection;
        private static IModel _channel;

        public RabbitMq(ILogger<RabbitMq> logger, IOptions<MqConfig> config)
        {
            _logger = logger;
            _config = config.Value;
        }

        public void Consume<T>(string routingKey, Func<T, bool> onReceive)
        {
            if (_connection == null)
            {
                var factory = new ConnectionFactory() { HostName = _config.HostName };
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
            }

            var exchangeName = "sahtivahti.recipes.fanout";
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, true);

            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queueName, exchangeName, routingKey);

            _logger.LogInformation(
                $"Consumer attached, exchange: {exchangeName}, queue: {queueName}, routingKey: {routingKey}"
            );

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                onReceive(
                    JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(ea.Body))
                );
            };

            _channel.BasicConsume(queueName, true, consumer);
        }
    }
}
