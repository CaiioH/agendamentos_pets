using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PetApi.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PetApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RacaController : ControllerBase
    {
        private readonly RacaService _service;

        public RacaController(RacaService service)
        {
            _service = service;
        }

        [HttpGet("listarRacas/")]
        [SwaggerOperation(
            Summary = "Busca uma lista de raças por especie",
            Description = "Retorna informações de todas as raças listadas"
        )]
        public async Task<IActionResult> ListarRacas([FromQuery] string especie)
        {
            try
            {
                var racas = await _service.ObterTodasRacas(especie);
                return Ok(racas);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao bucar raças: {ex.Message}");
            }
        }

        
        [HttpGet("BuscarImagemPorRaca/")]
        [SwaggerOperation(
            Summary = "Busca a raça por nome e por especie",
            Description = "Retorna informações da raça"
        )]
        public async Task<IActionResult> BuscarRacaPorNome([FromQuery] string nomeRaca, [FromQuery] string especie)
        {
            try
            {
                var raca = await _service.ObterRacaPorNome(nomeRaca, especie);
                var url = await _service.ObterImagemUrl(raca.ReferenceImageId, especie);
                return Ok(url);
            }
            catch (Exception e)
            {
                return BadRequest($"Erro ao bucar a raça: {e.Message}");
            }
        }

    }
}