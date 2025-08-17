using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PetApi.Infrastructure;
using PetApi.Services.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PetApi.Services
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly List<JsonElement> _messages;
        private readonly RabbitMqConnection _rabbitMqConnection;
        private readonly IServiceProvider _serviceProvider;

        public RabbitMqService(RabbitMqConnection rabbitMqConnection, IServiceProvider serviceProvider)
        {
            _rabbitMqConnection = rabbitMqConnection;
            _serviceProvider = serviceProvider;
            _messages = new List<JsonElement>();
        }

        public void SendMessage(string message, string queueName)
        {
            var channel = _rabbitMqConnection.GetChannel();

            var body = Encoding.UTF8.GetBytes(message);

            // Define a persistencia das Mensagens, caso o servidor reinicie, as mensagens continuam la
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: properties,
                body: body
            );

            Console.WriteLine("Mensagem enviada");
        }

        public void ConsumeMessages()
        {
            var channel = _rabbitMqConnection.GetChannel();

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var jsonMessage = JsonSerializer.Deserialize<JsonElement>(message);

                lock (_messages)
                {
                    //Adicionando mensagem
                    _messages.Add(jsonMessage);
                }

                ConsumirMensagemJson();
            };

            //Consome sempre que um Pet é criado.
            channel.BasicConsume(queue: "pet_info_request", autoAck: true, consumer: consumer);
        }

        public async Task<JsonElement?> ConsumirMensagemJson()
        {
            using var scope = _serviceProvider.CreateScope();
            var petService = scope.ServiceProvider.GetRequiredService<PetService>();

            List<JsonElement> mensagens;

            lock (_messages)
            {
                mensagens = new List<JsonElement>(_messages);
                _messages.Clear(); // Limpa a lista após pegar as mensagens

            }

            foreach (var message in mensagens)
            {
                var petId = 0;

                if (message.TryGetProperty("type", out JsonElement typeElement))
                {
                    string type = typeElement.GetString();
                    switch (type)
                    {
                        case "pet_info_request":
                            if (message.TryGetProperty("data", out JsonElement dataElement))
                            {
                                var nomePet = dataElement.GetProperty("NomePet").GetString();
                                var emailTutor = dataElement.GetProperty("EmailTutor").GetString();
                                var pet = await petService.BuscarPetPorIdRabbitMq(nomePet, emailTutor);
                                if (pet == null)
                                {
                                    var errorMsg = new
                                    {
                                        type = "pet_not_found",
                                        data = new
                                        {
                                            Error = "Pet não encontrado"
                                        }
                                    };

                                    string errorJsonString = JsonSerializer.Serialize(errorMsg);
                                    SendMessage(errorJsonString, "pet_info_response");
                                    return message;
                                }
                                Console.WriteLine($"O pet aq: {pet}");
                                var msg = new
                                {
                                    type = "pet_info_response",
                                    data = new
                                    {
                                        Id = pet.Id,
                                        NomePet = pet.Nome,
                                        Especie = pet.Tipo,
                                        Tutor = pet.Tutor,
                                        EmailTutor = pet.EmailTutor
                                    }
                                };

                                string jsonString = JsonSerializer.Serialize(msg);

                                SendMessage(jsonString, msg.type);

                            }
                            return message;

                        default:
                            Console.WriteLine("Mensagem não reconhecida");
                            break;
                    }
                }
            }
            return null;

        }

    }
}