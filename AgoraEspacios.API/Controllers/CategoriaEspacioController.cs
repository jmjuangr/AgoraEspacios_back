using AgoraEspacios.Business.Services;
using AgoraEspacios.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgoraEspacios.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaEspacioController : ControllerBase
    {
        private readonly CategoriaEspacioService _service;

        public CategoriaEspacioController(CategoriaEspacioService service)
        {
            _service = service;
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CategoriaEspacio>>> GetCategorias()
        {
            var categorias = await _service.GetAllAsync();
            return Ok(categorias);
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<CategoriaEspacio>> GetCategoria(int id)
        {
            var categoria = await _service.GetByIdAsync(id);
            if (categoria == null)
                return NotFound("Categoría no encontrada");

            return Ok(categoria);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateCategoria([FromBody] CategoriaEspacio categoria)
        {
            var result = await _service.CreateAsync(categoria);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateCategoria(int id, [FromBody] CategoriaEspacio categoria)
        {
            if (id != categoria.Id)
                return BadRequest("El ID no coincide.");

            var result = await _service.UpdateAsync(categoria);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteCategoria(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result.Message);
        }
    }
}
