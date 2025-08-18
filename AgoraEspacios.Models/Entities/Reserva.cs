using System.ComponentModel.DataAnnotations;

namespace AgoraEspacios.Models.Entities;

public class Reserva
{
    public int Id { get; set; }

    [Required]
    public int EspacioId { get; set; }

    [Required]
    public int UsuarioId { get; set; }

    [Required]
    public DateTime FechaInicio { get; set; }

    [Required]
    public DateTime FechaFin { get; set; }


    [Required, MaxLength(20)]
    public string Estado { get; set; } = "Activa";

    [MaxLength(200)]
    public string? Titulo { get; set; }


    public Espacio? Espacio { get; set; }
    public Usuario? Usuario { get; set; }
}
