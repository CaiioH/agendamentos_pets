using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgendaApi.Models;

namespace AgendaApi.Repository
{
    public interface IAgendamentoRepository
    {
        Task Add(Agenda agenda);
        Task Update(Agenda agenda);
        Task Delete(int id);
        Task DeletePorPetId(int petId);
        Task<List<Agenda>> ListarTodosAgendamentos();
        Task<Agenda?> BuscarAgendamentoPorId(int id);
        Task<List<Agenda>> ListarAgendamentosPorData(DateTime data);
        Task<List<Agenda>> ListarAgendamentosPorPetId(int petId);
        Task<int> BuscarQtAgendamentosDoDia(DateTime data);
        Task<List<Agenda>> BuscarAgendamentosPorEmail(string emailTutor);
        Task<List<Agenda>> BuscarAgendamentosPorServico(string servico);
        Task<List<Agenda>> BuscarAgendamentosPorData(DateTime dataAgendamento);

    }
}