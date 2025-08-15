using System.ComponentModel.DataAnnotations;

namespace AgoraEspacios.Models.DTOs
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }

        // "Admin" | "User"
        public string Rol { get; set; }
    }

    public class UsuarioCreateDTO
    {
        [Required, MinLength(3), MaxLength(80)]
        public string Nombre { get; set; }

        [Required, EmailAddress, MaxLength(120)]
        public string Email { get; set; }

        [Required, MinLength(6), MaxLength(100)]
        public string Password { get; set; }


        [Required, MaxLength(20)]
        public string Rol { get; set; }
    }

    public class UsuarioUpdateDTO
    {
        [Required, MinLength(3), MaxLength(80)]
        public string Nombre { get; set; }

        [Required, EmailAddress, MaxLength(120)]
        public string Email { get; set; }

        [Required, MaxLength(20)]
        public string Rol { get; set; }
    }
}
