using AgoraEspacios.Business.Services;
using AgoraEspacios.Models.DTOs;
using AgoraEspacios.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
            var existente = await _usuarioService.GetByEmailAsync(dto.Email);
            if (existente != null)
            {
                return BadRequest("El email ya esta registrado.");
            }

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                // guardar el hash de la contraseña no contraseña en texto plano
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = "User"
            };

            await _usuarioService.CreateAsync(usuario);

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
            // BCrypt compara la contraseña escrita con el hash guardado en bbd
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            {
                return Unauthorized("Credenciales invalidas.");
            }

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
            var clave = jwtConfig["Key"];

            //sii no hay clave no se pueden firmar token JWT
            if (string.IsNullOrWhiteSpace(clave))
            {
                throw new InvalidOperationException("No se ha configurado Jwt:Key.");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave));
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
