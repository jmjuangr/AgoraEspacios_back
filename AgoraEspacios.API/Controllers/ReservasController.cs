using AgoraEspacios.Business.Services;
using AgoraEspacios.Models.DTOs;
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

        [HttpGet("espacio/{espacioId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetByEspacio(int espacioId)
        {
            var reservas = await _reservaService.GetByEspacioAsync(espacioId);
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
        public async Task<IActionResult> Create([FromBody] ReservaCreateDTO dto)
        {
            var reserva = new Reserva
            {
                EspacioId = dto.EspacioId,
                UsuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                Titulo = dto.Titulo
            };

            var error = await _reservaService.CreateAsync(reserva);
            if (error != null) return BadRequest(error);

            return Ok(reserva);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ReservaUpdateDTO dto)
        {
            var reserva = await _reservaService.GetByIdAsync(id);
            if (reserva == null) return NotFound();

            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var esAdmin = User.IsInRole("Admin");

            if (!esAdmin && reserva.UsuarioId != usuarioId)
                return Forbid();

            reserva.FechaInicio = dto.FechaInicio;
            reserva.FechaFin = dto.FechaFin;
            reserva.Titulo = dto.Titulo;

            var error = await _reservaService.UpdateAsync(reserva);
            if (error != null) return BadRequest(error);

            return Ok(reserva);
        }

        [HttpPatch("{id}/aprobar")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Aprobar(int id)
        {
            var error = await _reservaService.AprobarAsync(id);
            if (error != null) return BadRequest(error);

            var reserva = await _reservaService.GetByIdAsync(id);
            return Ok(reserva);
        }

        [HttpPatch("{id}/rechazar")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Rechazar(int id)
        {
            var error = await _reservaService.RechazarAsync(id);
            if (error != null) return BadRequest(error);

            var reserva = await _reservaService.GetByIdAsync(id);
            return Ok(reserva);
        }

        [HttpPatch("{id}/cancelar")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Cancelar(int id)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var esAdmin = User.IsInRole("Admin");

            var error = await _reservaService.CancelarAsync(id, usuarioId, esAdmin);
            if (error != null) return BadRequest(error);

            var reserva = await _reservaService.GetByIdAsync(id);
            return Ok(reserva);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var esAdmin = User.IsInRole("Admin");

            var error = await _reservaService.CancelarAsync(id, usuarioId, esAdmin);
            if (error != null) return BadRequest(error);

            return NoContent();
        }
    }
}
