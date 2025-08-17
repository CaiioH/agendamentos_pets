using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PetApi.Services.Interfaces
{
    public interface IRabbitMqService
    {
        void ConsumeMessages();
        void SendMessage(string message, string queueName);
        Task<JsonElement?> ConsumirMensagemJson();
    }
}