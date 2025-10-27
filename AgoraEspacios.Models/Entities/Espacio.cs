using System.ComponentModel.DataAnnotations;

namespace AgoraEspacios.Models.Entities;

public class Espacio
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Nombre { get; set; } = default!;

    [Required]
    public int CategoriaEspacioId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La capacidad debe ser mayor que 0.")]
    public int Capacidad { get; set; }

    [MaxLength(150)]
    public string? Ubicacion { get; set; }

    [MaxLength(500)]
    public string? Descripcion { get; set; }

    // URL de imagen (para futura integración con almacenamiento)
    [MaxLength(300)]
    public string? ImagenUrl { get; set; }

    // Navegación
    public CategoriaEspacio? Categoria { get; set; }
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
