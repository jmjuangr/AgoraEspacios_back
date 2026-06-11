using AgoraEspacios.Business.Services;
using AgoraEspacios.Models.Entities;
using AgoraEspacios.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AgoraEspacios.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public UsuarioController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET api/usuario
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UsuarioDTO>>> GetAll()
        {
            var usuarios = await _usuarioService.GetAllAsync();
            var result = usuarios.ConvertAll(u => new UsuarioDTO
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Email = u.Email,
                Nif = u.Nif,
                Rol = u.Rol
            });

            return Ok(result);
        }

        // GET api/usuario/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UsuarioDTO>> GetById(int id)
        {
            var usuario = await _usuarioService.GetByIdAsync(id);
            if (usuario == null) return NotFound();

            var result = new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Nif = usuario.Nif,
                Rol = usuario.Rol
            };

            return Ok(result);
        }

        // POST api/usuario
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UsuarioDTO>> Create(UsuarioCreateDTO dto)
        {
            if (await _usuarioService.ExistsByNifAsync(dto.Nif))
            {
                return BadRequest("El NIF ya esta registrado.");
            }

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                Nif = dto.Nif,
                // Guardar la contraseña hasheada
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = dto.Rol
            };

            await _usuarioService.CreateAsync(usuario);

            var result = new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Nif = usuario.Nif,
                Rol = usuario.Rol
            };

            return Ok(result);
        }

        // PUT api/usuario/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(int id, UsuarioUpdateDTO dto)
        {
            var usuario = new Usuario
            {
                Id = id,
                Nombre = dto.Nombre,
                Email = dto.Email,
                Nif = dto.Nif,
                Rol = dto.Rol
            };

            var result = await _usuarioService.UpdateAsync(usuario);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        // DELETE api/usuario/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _usuarioService.DeleteAsync(id);
            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result.Message);
        }
    }
}
