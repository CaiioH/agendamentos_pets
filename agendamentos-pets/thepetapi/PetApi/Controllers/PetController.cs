using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PetApi.DTOs;
using PetApi.Repository;
using PetApi.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PetApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetController : ControllerBase
    {
        private readonly PetService _petService;

        public PetController(PetService petService)
        {
            _petService = petService;
        }

        [HttpPost("CriarPet")]
        [SwaggerOperation(
            Summary = "Criar um Pet",
            Description = "Cria um Pet de acordo com a Espécie e Raça"
        )]
        public async Task<IActionResult> CriarNovoPet([FromBody] PetDTO dto)
        {
            try
            {
                var pet = await _petService.CriarPet(dto);
                if(pet == null)
                {
                    return BadRequest(new { mensagem = "Erro ao criar pet, verifique os dados enviados." });
                }
                return CreatedAtAction(nameof(CriarNovoPet), new { id = pet.Nome }, pet);
            }
            catch (Exception e)
            {
                return BadRequest(new { mensagem = e.Message });
            }
        }

        [HttpPut("AtualizarPet/")]
        [SwaggerOperation(
            Summary = "Atualizar Pet",
            Description = "Atualiza as informações do Pet de acordo com o Id passado."
        )]
        public async Task<IActionResult> AtualizarPet([FromQuery] int id, [FromBody] PetDTO dto)
        {
            try
            {
                var pet = await _petService.AtualizarPet(id, dto);
                if(pet == null)
                {
                    return NotFound(new { mensagem = "Pet não encontrado, verifique os dados enviados." });
                }
                return Ok(pet);
            }
            catch (Exception e)
            {
                return BadRequest(new { mensagem = e.Message });
            }
        }

        [HttpDelete("DeletarPet/")]
        [SwaggerOperation(
            Summary = "Remover Pet",
            Description = "Remove o pet e suas informações do banco de dados."
        )]
        public async Task<IActionResult> DeletarPet([FromQuery] int id)
        {
            try
            {
                await _petService.DeletarPet(id);
                return Ok(new { mensagem = "Pet removido do banco de dados com sucesso." });
            }
            catch (Exception e)
            {
                return BadRequest(new { mensagem = e.Message });
            }
        }

        [HttpGet("ListarTodosPets")]
        [SwaggerOperation(
            Summary = "Listar todos os Pets",
            Description = "Retorna uma lista com todos os pets no banco de dados, incluindo suas informações"
        )]
        public async Task<IActionResult> ListarTodosPets()
        {
            try
            {
                var pets = await _petService.ListarTodosPets();
                return Ok(pets);
            }
            catch (Exception e)
            {
                return BadRequest(new { mensagem = e.Message });
            }
        }

        [HttpGet("ListarPetsPorEspecie/")]
        [SwaggerOperation(
            Summary = "Listar Pets por Espécie",
            Description = "Retorna todos os Pets e suas informações, de acordo com a Espécie"
        )]
        public async Task<IActionResult> ListarPetsPorEspecie([FromQuery] string especie)
        {
            try
            {
                var pets = await _petService.ListarPetsPorEspecie(especie);
                return Ok(pets);
            }
            catch (Exception e)
            {
                return BadRequest(new { mensagem = e.Message });
            }
        }

        [HttpGet("ListarPetsPorRaca/")]
        [SwaggerOperation(
            Summary = "Listar Pets por Raça",
            Description = "Retorna todos os Pets e suas informações, de acordo com a Raça"
        )]
        public async Task<IActionResult> ListarPetsPorRaca([FromQuery] string raca)
        {
            try
            {
                var pets = await _petService.ListarPetsPorRaca(raca);
                return Ok(pets);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("BuscarPetPorId/")]
        [SwaggerOperation(
            Summary = "Buscar Pet por Id",
            Description = "Retorna informações do Pet"
        )]
        public async Task<IActionResult> BuscarPetPorId([FromQuery] int id)
        {
            try
            {
                var pet = await _petService.BuscarPetPorId(id);
                if(pet == null)
                {
                    return NotFound("Pet não encontrado");
                }
                return Ok(pet);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
    }
}