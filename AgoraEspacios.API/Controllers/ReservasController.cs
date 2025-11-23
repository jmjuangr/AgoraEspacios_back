using AgoraEspacios.Business.Services;
using AgoraEspacios.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgoraEspacios.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {
        private readonly ReservaService _reservaService;

        public ReservasController(ReservaService reservaService)
        {
            _reservaService = reservaService;
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var reservas = await _reservaService.GetAllAsync();
            return Ok(reservas);
        }


        [HttpGet("mias")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetMisReservas()
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var reservas = await _reservaService.GetByUsuarioAsync(usuarioId);
            return Ok(reservas);
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var reserva = await _reservaService.GetByIdAsync(id);
            if (reserva == null) return NotFound();


            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var esAdmin = User.IsInRole("Admin");

            if (!esAdmin && reserva.UsuarioId != usuarioId)
                return Forbid();

            return Ok(reserva);
        }


        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Create([FromBody] Reserva reserva)
        {
            // asignar usuario actual
            reserva.UsuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var error = await _reservaService.CreateAsync(reserva);
            if (error != null) return BadRequest(error);

            return Ok(reserva);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Reserva reserva)
        {
            var actual = await _reservaService.GetByIdAsync(id);
            if (actual == null) return NotFound();

            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var esAdmin = User.IsInRole("Admin");

            if (!esAdmin && actual.UsuarioId != usuarioId)
                return Forbid();

            reserva.Id = id;
            reserva.UsuarioId = actual.UsuarioId;

            var error = await _reservaService.UpdateAsync(reserva);
            if (error != null) return BadRequest(error);

            return Ok(reserva);
        }

        // Eliminar reserva
        [HttpDelete("{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var reserva = await _reservaService.GetByIdAsync(id);
            if (reserva == null) return NotFound();

            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var esAdmin = User.IsInRole("Admin");

            if (!esAdmin && reserva.UsuarioId != usuarioId)
                return Forbid();

            await _reservaService.DeleteAsync(reserva);
            return NoContent();
        }
    }
}
