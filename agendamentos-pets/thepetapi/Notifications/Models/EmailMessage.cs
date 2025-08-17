using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notifications.Models
{
    public class EmailMessage
    {
        public string? Destinatario { get; set; }
        public string? Assunto { get; set; }
        public string? Mensagem { get; set; }
    }
}