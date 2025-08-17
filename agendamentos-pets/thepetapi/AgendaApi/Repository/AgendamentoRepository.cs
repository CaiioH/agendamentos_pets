using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgendaApi.Data;
using AgendaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AgendaApi.Repository
{
    public class AgendamentoRepository : IAgendamentoRepository
    {
        private readonly AppDbContext _context;

        public AgendamentoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Agenda agenda)
        {
            try
            {
                _context.Agendas.Add(agenda);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new Exception("Error ao adicionar agendamento ao banco de dados", ex);
            }
        }

        public async Task Update(Agenda agenda)
        {
            try
            {
                _context.Agendas.Update(agenda);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error ao atualizar agendamento no banco de dados", ex);
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                var agenda = await _context.Agendas.FindAsync(id);
                if (agenda == null)
                {
                    throw new Exception("Agendamento não encontrado");
                }
                _context.Agendas.Remove(agenda);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error ao remover agendamento do banco de dados", ex);
            }
        }

        public async Task DeletePorPetId(int petId)
        {
            try
            {
                var agendas = await _context.Agendas.Where(u => u.PetId == petId).ToListAsync();
                if (agendas == null)
                {
                    throw new Exception("Agendamento não encontrado");
                }
                _context.Agendas.RemoveRange(agendas);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error ao remover agendamento do banco de dados", ex);
            }
        }

        public async Task<List<Agenda>> ListarTodosAgendamentos()
        {
            try
            {
                return await _context.Agendas.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error ao listar agendamentos", ex);
            }
        }

        public async Task<Agenda?> BuscarAgendamentoPorId(int id)
        {
            try
            {
                return await _context.Agendas.FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error ao buscar agendamento por id", ex);
            }
        }

        public async Task<List<Agenda>> ListarAgendamentosPorData(DateTime data)
        {
            try
            {
                return await _context.Agendas.Where(u => u.DataAgendamento == data).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error ao buscar agendamento por data", ex);
            }
        }

        public async Task<List<Agenda>> ListarAgendamentosPorPetId(int petId)
        {
            try
            {
                return await _context.Agendas.Where(u => u.PetId == petId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error ao buscar agendamento por petId", ex);
            }
        }

        public async Task<int> BuscarQtAgendamentosDoDia(DateTime data)
        {
            try
            {
                return await _context.Agendas.CountAsync(u => u.DataAgendamento.Date == data);
            }
            catch (Exception ex)
            {
                throw new Exception("Error ao buscar quantidade de agendamentos do dia", ex);
            }
        }

        public async Task<List<Agenda>> BuscarAgendamentosPorEmail(string emailTutor)
        {
            try
            {
            return await _context.Agendas.Where(u => u.EmailTutor == emailTutor).ToListAsync();
            }
            catch (Exception ex)
            {
            throw new Exception("Error ao buscar agendamentos por email do tutor", ex);
            }
        }

        public async Task<List<Agenda>> BuscarAgendamentosPorServico(string servico)
        {
            try
            {
            return await _context.Agendas.Where(u => u.Servico == servico).ToListAsync();
            }
            catch (Exception ex)
            {
            throw new Exception("Error ao buscar agendamentos por serviço", ex);
            }
        }

        public async Task<List<Agenda>> BuscarAgendamentosPorData(DateTime dataAgendamento)
        {
            try
            {
            return await _context.Agendas.Where(u => u.DataAgendamento.Date == dataAgendamento.Date).ToListAsync();
            }
            catch (Exception ex)
            {
            throw new Exception("Error ao buscar agendamentos por data", ex);
            }
        }

    }
}