using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgendaApi.DTOs
{
    public class AgendamentoDTO
    {
        public string? Servico { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataAgendamento { get; set; }
    }
}