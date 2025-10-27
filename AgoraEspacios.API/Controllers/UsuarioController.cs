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
                Rol = usuario.Rol
            };

            return Ok(result);
        }

        // POST api/usuario
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UsuarioDTO>> Create(UsuarioCreateDTO dto)
        {
            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                PasswordHash = dto.Password,
                Rol = dto.Rol
            };

            await _usuarioService.CreateAsync(usuario);

            var result = new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol
            };

            return Ok(result);
        }
    }
}
