using AgoraEspacios.Data.Repositories;
using AgoraEspacios.Models.Entities;

namespace AgoraEspacios.Business.Services
{
    public class ReservaService
    {
        private const string EstadoPendiente = "Pendiente";
        private const string EstadoAprobada = "Aprobada";
        private const string EstadoRechazada = "Rechazada";
        private const string EstadoCancelada = "Cancelada";

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

        public async Task<List<Reserva>> GetByEspacioAsync(int espacioId)
        {
            return await _reservaRepo.GetByEspacioIdAsync(espacioId);
        }

        public async Task<Reserva?> GetByIdAsync(int id)
        {
            return await _reservaRepo.GetByIdAsync(id);
        }

        public async Task<string?> CreateAsync(Reserva reserva)
        {
            if (reserva.FechaFin <= reserva.FechaInicio)
                return "La fecha de fin debe ser posterior a la de inicio.";

            var espacio = await _espacioRepo.GetByIdAsync(reserva.EspacioId);
            if (espacio == null)
                return "El espacio indicado no existe.";

            bool solapa = await _reservaRepo.ExisteSolapamientoAprobadoAsync(
                reserva.EspacioId,
                reserva.FechaInicio,
                reserva.FechaFin
            );

            if (solapa)
                return "Ya existe una reserva aprobada en este espacio para el rango de fechas indicado.";

            reserva.Estado = EstadoPendiente;
            await _reservaRepo.AddAsync(reserva);
            return null;
        }

        public async Task<string?> UpdateAsync(Reserva reserva, bool esAdmin)
        {
            if (reserva.FechaFin <= reserva.FechaInicio)
                return "La fecha de fin debe ser posterior a la de inicio.";

            if (reserva.Estado == EstadoCancelada || reserva.Estado == EstadoRechazada)
                return "No se pueden modificar reservas canceladas o rechazadas.";

            if (reserva.Estado != EstadoPendiente && reserva.Estado != EstadoAprobada)
                return "Solo se pueden modificar reservas pendientes o aprobadas.";

            if (reserva.Estado == EstadoAprobada && !esAdmin)
            {
                reserva.Estado = EstadoPendiente;
            }

            if (reserva.Estado == EstadoAprobada)
            {
                bool solapa = await _reservaRepo.ExisteSolapamientoAprobadoAsync(
                    reserva.EspacioId,
                    reserva.FechaInicio,
                    reserva.FechaFin,
                    reserva.Id
                );

                if (solapa)
                    return "Ya existe otra reserva aprobada en este espacio para el rango de fechas indicado.";
            }

            await _reservaRepo.UpdateAsync(reserva);
            return null;
        }

        public async Task<string?> AprobarAsync(int reservaId)
        {
            var reserva = await _reservaRepo.GetByIdAsync(reservaId);
            if (reserva == null)
                return "La reserva no existe.";

            if (reserva.Estado != EstadoPendiente)
                return "Solo se pueden aprobar reservas pendientes.";

            bool solapa = await _reservaRepo.ExisteSolapamientoAprobadoAsync(
                reserva.EspacioId,
                reserva.FechaInicio,
                reserva.FechaFin,
                reserva.Id
            );

            if (solapa)
                return "Ya existe una reserva aprobada en este espacio para el rango de fechas indicado.";

            reserva.Estado = EstadoAprobada;
            await _reservaRepo.UpdateAsync(reserva);
            return null;
        }

        public async Task<string?> RechazarAsync(int reservaId)
        {
            var reserva = await _reservaRepo.GetByIdAsync(reservaId);
            if (reserva == null)
                return "La reserva no existe.";

            if (reserva.Estado != EstadoPendiente)
                return "Solo se pueden rechazar reservas pendientes.";

            reserva.Estado = EstadoRechazada;
            await _reservaRepo.UpdateAsync(reserva);
            return null;
        }

        public async Task<string?> CancelarAsync(int reservaId, int usuarioId, bool esAdmin)
        {
            var reserva = await _reservaRepo.GetByIdAsync(reservaId);
            if (reserva == null)
                return "La reserva no existe.";

            if (!esAdmin && reserva.UsuarioId != usuarioId)
                return "No puedes cancelar una reserva que no es tuya.";

            if (reserva.Estado == EstadoCancelada || reserva.Estado == EstadoRechazada)
                return "Esta reserva ya no se puede cancelar.";

            reserva.Estado = EstadoCancelada;
            await _reservaRepo.UpdateAsync(reserva);
            return null;
        }

        public async Task DeleteAsync(Reserva reserva)
        {
            await _reservaRepo.DeleteAsync(reserva);
        }
    }
}
