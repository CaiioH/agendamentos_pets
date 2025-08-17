using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AgendaApi.DTOs;
using AgendaApi.Infrastructure;
using AgendaApi.Models;
using AgendaApi.Repository;

namespace AgendaApi.Services
{
    public class AgendaService : IAgendaService
    {
        private readonly IAgendamentoRepository _agendamentoRepository;
        private readonly IRabbitMqService _rabbitMqService;

        public AgendaService(IAgendamentoRepository agendamentoRepository, IRabbitMqService rabbitMqService)
        {
            _agendamentoRepository = agendamentoRepository;
            _rabbitMqService = rabbitMqService;
        }

        public async Task<AgendamentoDTO> CriarAgendamentoAutomatico(int petId, string nomePet, string tutor, string emailTutor)
        {
            try
            {
                //preciso enviar a mensagem para a PetApi e esperar ela retornar o conteudo até aqui.
                var dataAgendamento = DateTime.Now.AddDays(7);

                // Se a data cair em um domingo, pula para o próximo dia (segunda-feira)
                if (dataAgendamento.DayOfWeek == DayOfWeek.Sunday)
                {
                    dataAgendamento = dataAgendamento.AddDays(1);
                }

                // Verifica se já existem 10 agendamentos para a data escolhida
                var agendamentosNoDia = await _agendamentoRepository.BuscarQtAgendamentosDoDia(dataAgendamento);
                while (agendamentosNoDia >= 10)
                {
                    dataAgendamento = dataAgendamento.AddDays(1);

                    // Se a nova data cair em um domingo, pula para o próximo dia (segunda-feira)
                    if (dataAgendamento.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dataAgendamento = dataAgendamento.AddDays(1);
                    }

                    agendamentosNoDia = await _agendamentoRepository.BuscarQtAgendamentosDoDia(dataAgendamento);
                }

                var agendamento = new Agenda
                {
                    Servico = "Banho Gratis",
                    Descricao = "Banho + Hidratação",
                    PetId = petId,
                    NomePet = nomePet,
                    Tutor = tutor,
                    EmailTutor = emailTutor,
                    DataCriacao = DateTime.Now,
                    DataAgendamento = dataAgendamento
                };

                await _agendamentoRepository.Add(agendamento);

                //Enviar a mensagem de Email aqui
                var msg = new
                {
                    Destinatario = agendamento.EmailTutor,
                    Assunto = "Agendamento de Pet",
                    Mensagem = $"<h1>Pet Agendado com Sucesso!</h1><br><br><h2>Seu pet {agendamento.NomePet} está agendado para {agendamento.DataAgendamento}. <br><br>Serviço: {agendamento.Servico}</h2>"
                };

                string jsonString = JsonSerializer.Serialize(msg);

                _rabbitMqService.SendMessage(jsonString, "appointment_created");

                return new AgendamentoDTO
                {
                    Servico = agendamento.Servico,
                    Descricao = agendamento.Descricao,
                    DataAgendamento = agendamento.DataAgendamento
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Agendamento Automatico negado: {e.Message}");
            }
        }

        public async Task<AgendamentoDTO> EfetuarAgendamentoManual(AgendamentoDTO dto, string petNome, string tutorEmail)
        {
            try
            {
                int petId = 0;
                string nomePet = string.Empty;
                string especie = string.Empty;
                string tutor = string.Empty;
                string emailTutor = string.Empty;

                var dataAgendamento = dto.DataAgendamento;
                // Se a data cair em um domingo, pula para o próximo dia (segunda-feira)
                if (dataAgendamento.DayOfWeek == DayOfWeek.Sunday)
                {
                    throw new Exception("Os agendamentos não são permitidos aos Domingos.");
                }

                // Confere a quantidade de agendamentos no dia escolhido pelo usuario
                var agendamentosNoDia = await _agendamentoRepository.BuscarQtAgendamentosDoDia(dataAgendamento);
                if (agendamentosNoDia >= 3)
                {
                    throw new Exception("Limite de agendamentos do dia Atingidos. Tente em outra data.");
                }

                JsonElement? mensagem = await BuscarInformaçãoViaRabbitMq(petNome, tutorEmail);
                Console.WriteLine($"Mensagem json: {mensagem.Value}");
                
                if (mensagem == null || mensagem.Value.GetProperty("type").GetString() == "pet_not_found")
                {
                    throw new Exception("Pet não encontrado.");
                }
                if (mensagem.Value.TryGetProperty("data", out JsonElement dataElement))
                {
                    petId = dataElement.GetProperty("Id").GetInt32();
                    nomePet = dataElement.GetProperty("NomePet").ToString();
                    especie = dataElement.GetProperty("Especie").ToString();
                    tutor = dataElement.GetProperty("Tutor").ToString();
                    emailTutor = dataElement.GetProperty("EmailTutor").ToString();
                }

                var agendamento = new Agenda
                {
                    Servico = dto.Servico,
                    Descricao = dto.Descricao,
                    PetId = petId,
                    NomePet = nomePet,
                    Tutor = tutor,
                    EmailTutor = emailTutor,
                    DataCriacao = DateTime.Now,
                    DataAgendamento = dataAgendamento
                };

                await _agendamentoRepository.Add(agendamento);

                //Enviar a mensagem de Email aqui
                var msg = new
                {
                    Destinatario = agendamento.EmailTutor,
                    Assunto = "Agendamento de Pet",
                    Mensagem = $"<h1>Pet Agendado com Sucesso!</h1><br><br><h2>Seu pet {agendamento.NomePet} está agendado para {agendamento.DataAgendamento}. <br><br>Serviço: {agendamento.Servico}</h2>"
                };

                string jsonString = JsonSerializer.Serialize(msg);

                _rabbitMqService.SendMessage(jsonString, "appointment_created");

                return new AgendamentoDTO
                {
                    Servico = agendamento.Servico,
                    Descricao = agendamento.Descricao,
                    DataAgendamento = agendamento.DataAgendamento
                };

            }
            catch (Exception e)
            {
                throw new Exception($"Agendamento manual negado: {e.Message}");
            }
        }

        public async Task<JsonElement?> BuscarInformaçãoViaRabbitMq(string nomePet, string emailTutor)
        {
            try
            {
                var message = new
                {
                    type = "pet_info_request",
                    data = new
                    {
                        NomePet = nomePet,
                        EmailTutor = emailTutor
                    }
                };

                string jsonString = JsonSerializer.Serialize(message);
                _rabbitMqService.SendMessage(jsonString, message.type);

                //Pega a menssagem em json
                await Task.Delay(500);

                //Aqui vai consumir atravez do metodo
                var mensagem = await _rabbitMqService.ConsumirMensagemJson();
                if (mensagem == null)
                {
                    throw new Exception("Nenhuma mensagem recebida da PetApi ou o pet não existe no banco de dados.");
                }

                return mensagem;

            }
            catch (Exception e)
            {
                throw new Exception($"Agendamento manual negado: {e.Message}");
            }
        }

        public async Task<AgendamentoDTO> AtualizarAgendamento(AgendamentoDTO dto, int id)
        {
            try
            {
                var agendamento = await _agendamentoRepository.BuscarAgendamentoPorId(id);
                if (agendamento == null)
                {
                    throw new Exception("Agendamento não encontrado");
                }

                agendamento.Servico = string.IsNullOrEmpty(dto.Servico) ? agendamento.Servico : dto.Servico;
                agendamento.Descricao = string.IsNullOrEmpty(dto.Descricao) ? agendamento.Descricao : dto.Descricao;
                agendamento.DataAgendamento = dto.DataAgendamento == default(DateTime) ? agendamento.DataAgendamento : dto.DataAgendamento;
                agendamento.DataCriacao = DateTime.Now;

                await _agendamentoRepository.Update(agendamento);

                return new AgendamentoDTO
                {
                    Servico = agendamento.Servico,
                    Descricao = agendamento.Descricao,
                    DataAgendamento = agendamento.DataAgendamento
                };

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task DeletarAgendamento(int id)
        {
            try
            {
                await _agendamentoRepository.Delete(id);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task DeletarAgendamentoPorPetId(int petId)
        {
            try
            {
                await _agendamentoRepository.DeletePorPetId(petId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<Agenda>> ListarTodosAgendamentos()
        {
            try
            {
                var agendamentos = await _agendamentoRepository.ListarTodosAgendamentos();
                if (agendamentos == null)
                {
                    throw new Exception("Nenhum agendamento encontrado");
                }
                return agendamentos.Select(agendamento => new Agenda
                {
                    Id = agendamento.Id,
                    Servico = agendamento.Servico,
                    Descricao = agendamento.Descricao,
                    PetId = agendamento.PetId,
                    NomePet = agendamento.NomePet,
                    Tutor = agendamento.Tutor,
                    EmailTutor = agendamento.EmailTutor,
                    DataAgendamento = agendamento.DataAgendamento,
                    DataCriacao = agendamento.DataCriacao
                }).ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar Agendamentos no banco de dados: {e.Message}");
            }
        }

        public async Task<List<Agenda>> ListarAgendamentoPorEmail(string emailTutor)
        {
            try
            {
                var agendamentos = await _agendamentoRepository.BuscarAgendamentosPorEmail(emailTutor);
                if (agendamentos == null || !agendamentos.Any())
                {
                    throw new Exception("Nenhum agendamento encontrado para o email fornecido.");
                }
                return agendamentos;
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar agendamentos por email: {e.Message}");
            }
        }

        public async Task<List<Agenda>> ListarAgendamentoPorServico(string servico)
        {
            try
            {
                var agendamentos = await _agendamentoRepository.BuscarAgendamentosPorServico(servico);
                if (agendamentos == null || !agendamentos.Any())
                {
                    throw new Exception("Nenhum agendamento encontrado para o serviço fornecido.");
                }
                return agendamentos;
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar agendamentos por serviço: {e.Message}");
            }
        }

        public async Task<List<Agenda>> ListarAgendamentoPorData(DateTime dataAgendamento)
        {
            try
            {
                var agendamentos = await _agendamentoRepository.BuscarAgendamentosPorData(dataAgendamento);
                if (agendamentos == null || !agendamentos.Any())
                {
                    throw new Exception("Nenhum agendamento encontrado para a data fornecida.");
                }
                return agendamentos;
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar agendamentos por data: {e.Message}");
            }
        }

        public async Task<Agenda> ListarAgendamentoPorId(int id)
        {
            try
            {
                var agendamento = await _agendamentoRepository.BuscarAgendamentoPorId(id);
                if (agendamento == null)
                {
                    throw new Exception("Nenhum agendamento encontrado para o id fornecido.");
                }
                return agendamento;
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar agendamento por id: {e.Message}");
            }
        }
    }

}