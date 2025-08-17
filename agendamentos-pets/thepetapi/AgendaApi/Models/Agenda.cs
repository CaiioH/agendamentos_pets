using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgendaApi.Models
{
    public class Agenda
    {
        public int Id { get; set; }
        public string? Servico { get; set; }
        public string? Descricao { get; set; }
        public int PetId { get; set; }
        public string? NomePet { get; set; }

        public string? Tutor { get; set; }
        public string? EmailTutor { get; set; }

        public DateTime DataCriacao { get; set; }
        public DateTime DataAgendamento { get; set; }

    }
}