using System;
using System.ComponentModel.DataAnnotations;

namespace AgoraEspacios.Models.DTOs
{
    public class ReservaDTO
    {
        public int Id { get; set; }

        public int EspacioId { get; set; }
        public string EspacioNombre { get; set; } = string.Empty;

        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaCreacion { get; set; }

        // "Pendiente" | "Aprobada" | "Rechazada" | "Cancelada"
        public string Estado { get; set; } = string.Empty;

        public string? Titulo { get; set; }
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
        public string? Titulo { get; set; }
    }

    public class ReservaUpdateDTO
    {
        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        [MaxLength(200)]
        public string? Titulo { get; set; }
    }
}
