using AgoraEspacios.Business.Services;
using AgoraEspacios.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgoraEspacios.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EspacioController : ControllerBase
    {
        private readonly EspacioService _service;

        public EspacioController(EspacioService service)
        {
            _service = service;
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Espacio>>> GetEspacios()
        {
            var espacios = await _service.GetAllAsync();
            return Ok(espacios);
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Espacio>> GetEspacio(int id)
        {
            var espacio = await _service.GetByIdAsync(id);
            if (espacio == null)
                return NotFound("Espacio no encontrado");

            return Ok(espacio);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateEspacio([FromBody] Espacio espacio)
        {
            var result = await _service.CreateAsync(espacio);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateEspacio(int id, [FromBody] Espacio espacio)
        {
            if (id != espacio.Id)
                return BadRequest("El ID no coincide.");

            var result = await _service.UpdateAsync(espacio);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteEspacio(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result.Message);
        }
    }
}
