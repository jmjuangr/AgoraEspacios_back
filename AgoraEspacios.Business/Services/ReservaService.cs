using AgoraEspacios.Data.Repositories;
using AgoraEspacios.Models.Entities;

namespace AgoraEspacios.Business.Services
{
    public class ReservaService
    {
        private readonly ReservaRepository _reservaRepo;
        private readonly EspacioRepository _espacioRepo;

        public ReservaService(ReservaRepository reservaRepo, EspacioRepository espacioRepo)
        {
            _reservaRepo = reservaRepo;
            _espacioRepo = espacioRepo;
        }


        public async Task<List<Reserva>> GetAllAsync()
        {
            return await _reservaRepo.GetAllAsync();
        }


        public async Task<List<Reserva>> GetByUsuarioAsync(int usuarioId)
        {
            return await _reservaRepo.GetByUsuarioIdAsync(usuarioId);
        }


        public async Task<Reserva?> GetByIdAsync(int id)
        {
            return await _reservaRepo.GetByIdAsync(id);
        }

        // Crear reserva con validaciones
        public async Task<string?> CreateAsync(Reserva reserva)
        {
            // Validar fechas
            if (reserva.FechaFin <= reserva.FechaInicio)
                return "La fecha de fin debe ser posterior a la de inicio.";

            // Cargar el espacio
            var espacio = await _espacioRepo.GetByIdAsync(reserva.EspacioId);
            if (espacio == null)
                return "El espacio indicado no existe.";

            // Si el espacio necesita aprobación manual entonces reserva Pendiente
            if (espacio.RequiereAprobacionAdmin)
            {
                reserva.Estado = "Pendiente";
                await _reservaRepo.AddAsync(reserva);
                return null;
            }

            // Si NO necesita aprobación manual:

            bool solapa = await _reservaRepo.ExisteSolapamientoAsync(
                reserva.EspacioId,
                reserva.FechaInicio,
                reserva.FechaFin
            );

            if (solapa)
                return "Ya existe una reserva aprobada en este espacio para el rango de fechas indicado.";

            reserva.Estado = "Aprobada";
            await _reservaRepo.AddAsync(reserva);
            return null;
        }


        // Editar reserva 
        public async Task<string?> UpdateAsync(Reserva reserva)
        {
            if (reserva.FechaFin <= reserva.FechaInicio)
                return "La fecha de fin debe ser posterior a la de inicio.";

            bool solapa = await _reservaRepo.ExisteSolapamientoAsync(reserva.EspacioId, reserva.FechaInicio, reserva.FechaFin, reserva.Id);
            if (solapa)
                return "Ya existe otra reserva en este espacio para el rango de fechas indicado.";

            await _reservaRepo.UpdateAsync(reserva);
            return null;
        }


        public async Task DeleteAsync(Reserva reserva)
        {
            await _reservaRepo.DeleteAsync(reserva);
        }
    }
}
