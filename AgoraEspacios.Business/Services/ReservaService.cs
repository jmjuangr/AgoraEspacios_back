using AgoraEspacios.Data.Repositories;
using AgoraEspacios.Models.Entities;

namespace AgoraEspacios.Business.Services
{
    public class ReservaService
    {
        private readonly ReservaRepository _reservaRepo;

        public ReservaService(ReservaRepository reservaRepo)
        {
            _reservaRepo = reservaRepo;
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

            // Validar solapamientos
            bool solapa = await _reservaRepo.ExisteSolapamientoAsync(reserva.EspacioId, reserva.FechaInicio, reserva.FechaFin);
            if (solapa)
                return "Ya existe una reserva en este espacio para el rango de fechas indicado.";

            await _reservaRepo.AddAsync(reserva);
            return null; // null = sin errores
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
