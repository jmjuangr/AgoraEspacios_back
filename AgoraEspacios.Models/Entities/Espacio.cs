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

    // Indica si el espacio es accesible 
    public bool EsAccesible { get; set; } = false;

    // Indica si este espacio requiere aprobación manual del admin para cada reserva
    public bool RequiereAprobacionAdmin { get; set; } = false;

    // URL de imagen 
    [MaxLength(300)]
    public string? ImagenUrl { get; set; }

    // Navegación
    public CategoriaEspacio? Categoria { get; set; }
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
