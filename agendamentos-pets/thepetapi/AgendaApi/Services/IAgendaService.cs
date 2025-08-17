using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AgendaApi.DTOs;
using AgendaApi.Models;

namespace AgendaApi.Services
{
    public interface IAgendaService
    {
        Task<AgendamentoDTO> CriarAgendamentoAutomatico(int petId,  string nomePet, string tutor, string emailTutor);
        Task<JsonElement?> BuscarInformaçãoViaRabbitMq(string nomePet, string emailTutor);
        Task<AgendamentoDTO> EfetuarAgendamentoManual(AgendamentoDTO dto, string nomePet, string emailTutor);
        Task<AgendamentoDTO> AtualizarAgendamento(AgendamentoDTO dto, int id);
        Task DeletarAgendamento(int id);
        Task DeletarAgendamentoPorPetId(int petId);
        Task<List<Agenda>> ListarTodosAgendamentos();
        Task<Agenda> ListarAgendamentoPorId(int id);
        Task<List<Agenda>> ListarAgendamentoPorEmail(string emailTutor);
        Task<List<Agenda>> ListarAgendamentoPorServico(string servico);
        Task<List<Agenda>> ListarAgendamentoPorData(DateTime dataAgendamento);
    }
}