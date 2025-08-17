using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetApi.Models
{
    public class Pet
    {
        public int Id { get; set; }

        public string Tutor { get; set; }
        public string EmailTutor { get; set; }

        public string? Nome { get; set; }
        public string Cor { get; set; } = string.Empty;
        public string? Sexo { get; set; }
        public string? Idade { get; set; }
        public string? Tipo { get; set; }
        public string? RacaId { get; set; }
        public string? NomeRaca { get; set; }
        public string? ImagemUrl { get; set; }

    }
}