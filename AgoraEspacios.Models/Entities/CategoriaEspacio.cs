using System.ComponentModel.DataAnnotations;

namespace AgoraEspacios.Models.Entities;

public class CategoriaEspacio
{
    public int Id { get; set; }

    [Required, MaxLength(60)]
    public string Nombre { get; set; } = default!; // Único (índice único en Data)

    [MaxLength(200)]
    public string? Descripcion { get; set; }


    public ICollection<Espacio> Espacios { get; set; } = new List<Espacio>();
}
