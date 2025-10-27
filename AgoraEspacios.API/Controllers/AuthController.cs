using AgoraEspacios.Business.Services;
using AgoraEspacios.Models.DTOs;
using AgoraEspacios.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace AgoraEspacios.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;
        private readonly IConfiguration _configuration;

        public AuthController(UsuarioService usuarioService, IConfiguration configuration)
        {
            _usuarioService = usuarioService;
            _configuration = configuration;
        }

        // POST api/auth/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest dto)
        {
            // Verificar si ya existe un usuario con ese email
            var existente = await _usuarioService.GetByEmailAsync(dto.Email);
            if (existente != null)
            {
                return BadRequest("El email ya está registrado.");
            }

            // Crear usuario nuevo (rol por defecto = User)
            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                PasswordHash = dto.Password,
                Rol = "User"
            };

            await _usuarioService.CreateAsync(usuario);

            // Generar token
            var tokenString = GenerateJwtToken(usuario);

            var response = new AuthResponse
            {
                UsuarioId = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol,
                Token = tokenString,
                ExpiresAt = DateTime.UtcNow.AddHours(2)
            };

            return Ok(response);
        }

        // POST api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest dto)
        {
            var usuario = await _usuarioService.GetByEmailAsync(dto.Email);
            if (usuario == null || usuario.PasswordHash != dto.Password)
            {
                return Unauthorized("Credenciales inválidas.");
            }

            // Generar token
            var tokenString = GenerateJwtToken(usuario);

            var response = new AuthResponse
            {
                UsuarioId = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol,
                Token = tokenString,
                ExpiresAt = DateTime.UtcNow.AddHours(2)
            };

            return Ok(response);
        }


        private string GenerateJwtToken(Usuario usuario)
        {
            var jwtConfig = _configuration.GetSection("Jwt");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtConfig["Issuer"],
                audience: jwtConfig["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
