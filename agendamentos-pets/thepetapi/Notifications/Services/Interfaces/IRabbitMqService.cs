using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notifications.Services.Interfaces
{
    public interface IRabbitMqService
    {
        void ConsumeMessages();
    }
}