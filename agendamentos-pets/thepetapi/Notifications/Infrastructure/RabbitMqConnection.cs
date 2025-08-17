using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;

namespace Notifications.Infrastructure
{
    public class RabbitMqConnection
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqConnection(IConfiguration configuration)
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:HostName"],
                Port = 5672,
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };

            _connection = factory.CreateConnection();

            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: "appointment_created",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

        }

        public IModel GetChannel()
        {
            return _channel;
        }
    }
}