using System;
using System.ComponentModel.DataAnnotations;

namespace AgoraEspacios.Models.DTOs
{
    public class RegisterRequest
    {
        [Required, MinLength(3), MaxLength(80)]
        public string Nombre { get; set; }

        [Required, EmailAddress, MaxLength(120)]
        public string Email { get; set; }

        [Required, MinLength(6), MaxLength(100)]
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        [Required, EmailAddress, MaxLength(120)]
        public string Email { get; set; }

        [Required, MinLength(6), MaxLength(100)]
        public string Password { get; set; }
    }

    public class AuthResponse
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }


        public string Rol { get; set; }

        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
