using System;
using System.ComponentModel.DataAnnotations;

namespace AgoraEspacios.Models.DTOs
{
    public class ReservaDTO
    {
        public int Id { get; set; }

        public int EspacioId { get; set; }
        public string EspacioNombre { get; set; }

        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        // "Pendiente" | "Aprobada" | "Rechazada" | "Cancelada"
        public string Estado { get; set; }

        public string Titulo { get; set; }
    }

    public class ReservaCreateDTO
    {
        [Required]
        public int EspacioId { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        [MaxLength(200)]
        public string Titulo { get; set; }
    }

    public class ReservaUpdateDTO
    {
        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        [MaxLength(200)]
        public string Titulo { get; set; }

        [Required, MaxLength(20)]
        public string Estado { get; set; }
    }
}
