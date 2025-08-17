using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AgendaApi.Services
{
    public interface IRabbitMqService
    {
        void ConsumeMessages();
        void SendMessage(string message, string queueName);
        Task<JsonElement?> ConsumirMensagemJson();
    }
}