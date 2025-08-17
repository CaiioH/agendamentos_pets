using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Notifications.Infrastructure;
using Notifications.Models;
using Notifications.Services.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Notifications.Services
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly RabbitMqConnection _rabbitMqConnection;
        private readonly IEmailService _emailService;

        public RabbitMqService(RabbitMqConnection rabbitMqConnection, IEmailService emailService)
        {
            _rabbitMqConnection = rabbitMqConnection;
            _emailService = emailService;
        }

        public async void ConsumeMessages()
        {
            var channel = _rabbitMqConnection.GetChannel();

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);

                var jsonMessage = JsonSerializer.Deserialize<EmailMessage>(message);
                if (jsonMessage != null)
                {
                    if (string.IsNullOrWhiteSpace(jsonMessage.Destinatario))
                    {
                        // Log ou lançar uma exceção se o destinatário estiver vazio
                        throw new ArgumentException("O endereço de e-mail do destinatário não pode ser nulo ou vazio.");
                    }
                    await _emailService.EnviarEmailAsync(jsonMessage.Destinatario, jsonMessage.Assunto, jsonMessage.Mensagem);
                }

                channel.BasicAck(ea.DeliveryTag, false);

            };

            //Consome sempre que um Servico é criado.
            channel.BasicConsume(queue: "appointment_created", autoAck: false, consumer: consumer);

        }
    }
}