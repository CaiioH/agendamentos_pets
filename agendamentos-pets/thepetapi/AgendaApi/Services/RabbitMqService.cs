using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AgendaApi.Infrastructure;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AgendaApi.Services
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

        public async void ConsumeMessages()
        {
            var channel = _rabbitMqConnection.GetChannel();

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);

                var jsonMessage = JsonSerializer.Deserialize<JsonElement>(message);
                lock (_messages)
                {
                    _messages.Add(jsonMessage);
                }

                await Task.Delay(1500);
                await ConsumirMensagemJson();
            };


            //Consome sempre que um Pet é criado.
            channel.BasicConsume(queue: "pet_created", autoAck: true, consumer: consumer);

            //Consome aqui pra receber as informações pedidas pro cadastro manual.
            channel.BasicConsume(queue: "pet_info_response", autoAck: true, consumer: consumer);
            
            //Consome aqui pra receber as informações do pet deletado.
            channel.BasicConsume(queue: "pet_deleted", autoAck: true, consumer: consumer);

        }

        public async Task<JsonElement?> ConsumirMensagemJson()
        {
            using var scope = _serviceProvider.CreateScope();
            var agendaService = scope.ServiceProvider.GetRequiredService<IAgendaService>();

            List<JsonElement> mensagens;

            lock (_messages)
            {
                mensagens = new List<JsonElement>(_messages);

                _messages.Clear(); // Limpa a lista após pegar as mensagens

            }
            foreach (var message in mensagens)
            {
                if (message.TryGetProperty("type", out JsonElement typeElement))
                {
                    string type = typeElement.GetString();
                    switch (type)
                    {
                        case "pet_created":
                            if (message.TryGetProperty("data", out JsonElement dataElement))
                            {
                                var petId = dataElement.GetProperty("Id").GetInt32();
                                var nomePet = dataElement.GetProperty("Nome").GetString();
                                var especie = dataElement.GetProperty("Especie").GetString();
                                var tutor = dataElement.GetProperty("Tutor").GetString();
                                var emailTutor = dataElement.GetProperty("EmailTutor").GetString();

                                await agendaService.CriarAgendamentoAutomatico(petId, nomePet, tutor, emailTutor);
                            }
                            return message;

                        case "pet_info_response":
                            return message;
                        
                        case "pet_deleted":
                            if (message.TryGetProperty("data", out JsonElement dElement))
                            {
                                var idPet = dElement.GetProperty("PetId").GetInt32();
                                Console.WriteLine($"Id do pet deletado: {idPet}");
                                await agendaService.DeletarAgendamentoPorPetId(idPet);
                            }
                            return message;

                        default:
                            break;
                    }
                }
            }
            return default;

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
            
        }

    }

}