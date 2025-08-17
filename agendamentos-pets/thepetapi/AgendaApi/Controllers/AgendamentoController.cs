using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgendaApi.DTOs;
using AgendaApi.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;


namespace AgendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgendamentoController : ControllerBase
    {
        private readonly IAgendaService _agendaService;

        public AgendamentoController(IAgendaService agendaService)
        {
            _agendaService = agendaService;
        }

        [HttpPost("CriarAgendamentoManual/")]
        [SwaggerOperation(
            Summary = "Criar Agendamento Manualmente",
            Description = "Busca informações na PetApi e retorna para fazer o agendamento manualmente"
        )]
        public async Task<IActionResult> CriarAgendamentoManual([FromBody] AgendamentoDTO dto, [FromQuery] string nomePet, [FromQuery] string emailTutor)
        {
            try
            {
                var agendamento = await _agendaService.EfetuarAgendamentoManual(dto, nomePet, emailTutor);
                if (agendamento == null)
                {
                    return NotFound(new { mensagem = "Erro ao fazer Agendamento manual, verifique os dados enviados." });
                }
                return CreatedAtAction(nameof(CriarAgendamentoManual), new { Servico = agendamento.Servico, Agendado = agendamento.DataAgendamento, Criado = DateTime.Now }, agendamento);
            }
            catch (Exception e)
            {
                return BadRequest(new { mensagem = e.Message });
            }

        }

        [HttpPut("AtualizarAgendamento/")]
        [SwaggerOperation(
            Summary = "Atualizar Agendamento",
            Description = "Atualiza as informações do Agendamento"
        )]
        public async Task<IActionResult> AtualizarAgendamento([FromBody] AgendamentoDTO dto, [FromQuery] int id)
        {
            try
            {
                var agendamento = await _agendaService.AtualizarAgendamento(dto, id);
                if (agendamento == null)
                {
                    return BadRequest(new { mensagem = "Erro ao atualizar o agendamento, verifique os dados enviados" });
                }
                return Ok(agendamento);
            }
            catch (Exception e)
            {
                return BadRequest(new { mensagem = e.Message });
            }
        }

        [HttpDelete("DeletarAgendamento/")]
        [SwaggerOperation(
                    Summary = "Deletar Agendamento",
                    Description = "Deleta agendamento e suas informações do banco de dados."
                )]
        public async Task<IActionResult> DeletarAgendamento([FromQuery] int id)
        {
            try
            {
                await _agendaService.DeletarAgendamento(id);
                return Ok(new { mensagem = "Agendamento removido do banco de dados com sucesso." });
            }
            catch (Exception e)
            {
                return BadRequest(new { mensagem = e.Message });
            }
        }

        [HttpGet("ListarTodosAgendamentos")]
        [SwaggerOperation(
            Summary = "Listar Todos os Agendamentos",
            Description = "Lista todos os agendamentos"
        )]
        public async Task<IActionResult> ListarTodosAgendamentos()
        {
            try
            {
                var agendamentos = await _agendaService.ListarTodosAgendamentos();
                return Ok(agendamentos);
            }
            catch (Exception e)
            {
                return BadRequest(new { mensagem = e.Message });
            }
        }

        [HttpGet("ListarAgendamentoPorEmail/")]
        [SwaggerOperation(
            Summary = "Listar Agendamento por Email",
            Description = "Lista agendamentos por email do tutor"
        )]
        public async Task<IActionResult> ListarAgendamentoPorEmail([FromQuery] string email)
        {
            try
            {
                var agendamentos = await _agendaService.ListarAgendamentoPorEmail(email);
                return Ok(agendamentos);
            }
            catch (Exception e)
            {
                return BadRequest(new { mensagem = e.Message });
            }
        }

        [HttpGet("ListarAgendamentoPorServico/")]
        [SwaggerOperation(
            Summary = "Listar Agendamento por Serviço",
            Description = "Lista agendamentos por tipo de serviço"
        )]
        public async Task<IActionResult> ListarAgendamentoPorServico([FromQuery] string servico)
        {
            try
            {
                var agendamentos = await _agendaService.ListarAgendamentoPorServico(servico);
                return Ok(agendamentos);
            }
            catch (Exception e)
            {
                return BadRequest(new { mensagem = e.Message });
            }
        }

        [HttpGet("ListarAgendamentoPorData/")]
        [SwaggerOperation(
            Summary = "Listar Agendamento por Data",
            Description = "Lista agendamentos por data (EX: 2025-03-12)"
        )]
        public async Task<IActionResult> ListarAgendamentoPorData([FromQuery] DateTime data)
        {
            try
            {
                var agendamentos = await _agendaService.ListarAgendamentoPorData(data);
                return Ok(agendamentos);
            }
            catch (Exception e)
            {
                return BadRequest(new { mensagem = e.Message });
            }
        }

        [HttpGet("BuscarAgendamentoPorId/")]
        [SwaggerOperation(
            Summary = "Buscar Agendamento por Id",
            Description = "Busca agendamentos por Id"
        )]
        public async Task<IActionResult> ListarAgendamentoPorId([FromQuery] int id)
        {
            try
            {
                var agendamento = await _agendaService.ListarAgendamentoPorId(id);
                return Ok(agendamento);
            }
            catch (Exception e)
            {
                return BadRequest(new { mensagem = e.Message });
            }
        }
    }
}