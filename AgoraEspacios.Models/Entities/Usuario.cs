using System.ComponentModel.DataAnnotations;

namespace AgoraEspacios.Models.Entities;

public class Usuario
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Nombre { get; set; } = default!;

    [Required, EmailAddress, MaxLength(120)]
    public string Email { get; set; } = default!;

    [Required]
    public string PasswordHash { get; set; } = default!;

    [Required, MaxLength(20)]
    public string Rol { get; set; } = "User";


    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
