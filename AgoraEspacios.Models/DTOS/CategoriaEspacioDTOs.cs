using System.ComponentModel.DataAnnotations;

namespace AgoraEspacios.Models.DTOs
{
    public class CategoriaEspacioDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }

    public class CategoriaEspacioCreateDTO
    {
        [Required, MinLength(3), MaxLength(60)]
        public string Nombre { get; set; }

        [MaxLength(200)]
        public string Descripcion { get; set; }
    }

    public class CategoriaEspacioUpdateDTO
    {
        [Required, MinLength(3), MaxLength(60)]
        public string Nombre { get; set; }

        [MaxLength(200)]
        public string Descripcion { get; set; }
    }
}
