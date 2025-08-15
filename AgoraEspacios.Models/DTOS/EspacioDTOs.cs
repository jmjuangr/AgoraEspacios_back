using System.ComponentModel.DataAnnotations;

namespace AgoraEspacios.Models.DTOs
{
    public class EspacioDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public int CategoriaEspacioId { get; set; }
        public string CategoriaNombre { get; set; }

        public int Capacidad { get; set; }
        public string Ubicacion { get; set; }
        public string Descripcion { get; set; }
        public string ImagenUrl { get; set; }
    }

    public class EspacioCreateDTO
    {
        [Required, MinLength(3), MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        public int CategoriaEspacioId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La capacidad debe ser > 0")]
        public int Capacidad { get; set; }

        [MaxLength(150)]
        public string Ubicacion { get; set; }

        [MaxLength(500)]
        public string Descripcion { get; set; }

        [MaxLength(300)]
        public string ImagenUrl { get; set; }
    }

    public class EspacioUpdateDTO
    {
        [Required, MinLength(3), MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        public int CategoriaEspacioId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La capacidad debe ser > 0")]
        public int Capacidad { get; set; }

        [MaxLength(150)]
        public string Ubicacion { get; set; }

        [MaxLength(500)]
        public string Descripcion { get; set; }

        [MaxLength(300)]
        public string ImagenUrl { get; set; }
    }
}
