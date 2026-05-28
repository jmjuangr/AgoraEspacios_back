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
            //Devuelvo reservas
            return await _reservaRepo.GetAllAsync();
        }

        public async Task<List<Reserva>> GetByUsuarioAsync(int usuarioId)
        {
            // buscar reservas usuario
            return await _reservaRepo.GetByUsuarioIdAsync(usuarioId);
        }

        public async Task<List<Reserva>> GetByEspacioAsync(int espacioId)
        {
            //buscar reservas de un espacio
            return await _reservaRepo.GetByEspacioIdAsync(espacioId);
        }

        public async Task<Reserva?> GetByIdAsync(int id)
        {
            //buscar reservas por id
            return await _reservaRepo.GetByIdAsync(id);
        }

        public async Task<string?> CreateAsync(Reserva reserva)
        {
            // validacion fechafinal posterior a fecha inicial
            if (reserva.FechaFin <= reserva.FechaInicio)
                return "La fecha de fin debe ser posterior a la de inicio.";

            // cimprobar que el espacio existe
            var espacio = await _espacioRepo.GetByIdAsync(reserva.EspacioId);
            if (espacio == null)
                return "El espacio indicado no existe.";

            // ver reserva aprobada que coincida en fecha
            bool solapa = await _reservaRepo.ExisteSolapamientoAprobadoAsync(
                reserva.EspacioId,
                reserva.FechaInicio,
                reserva.FechaFin
            );

            if (solapa)
                return "Ya existe una reserva aprobada en este espacio para el rango de fechas indicado.";

            // Las reservas nuevas empiezan como pendientes hasta revision de admin
            reserva.Estado = EstadoPendiente;
            await _reservaRepo.AddAsync(reserva);
            return null;
        }

        public async Task<string?> UpdateAsync(Reserva reserva, bool esAdmin)
        {
            // Vuelvo a validar las fechas cuando se modifica la reserva
            if (reserva.FechaFin <= reserva.FechaInicio)
                return "La fecha de fin debe ser posterior a la de inicio.";

            // No permito tocar reservas que ya terminaron como canceladas o rechazadas.
            if (reserva.Estado == EstadoCancelada || reserva.Estado == EstadoRechazada)
                return "No se pueden modificar reservas canceladas o rechazadas.";

            // limitar cambios en estados 
            if (reserva.Estado != EstadoPendiente && reserva.Estado != EstadoAprobada)
                return "Solo se pueden modificar reservas pendientes o aprobadas.";

            // si usuario modifica la reserva vuelve a estadoi pendiente
            if (reserva.Estado == EstadoAprobada && !esAdmin)
            {
                reserva.Estado = EstadoPendiente;
            }

            // si se aprueba comprobar que no se sola con otra aprobada
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
            // buscar reservas que se quiere aprobar
            var reserva = await _reservaRepo.GetByIdAsync(reservaId);
            if (reserva == null)
                return "La reserva no existe.";

            // solo se aprueba si esta pendiente
            if (reserva.Estado != EstadoPendiente)
                return "Solo se pueden aprobar reservas pendientes.";

            // solapamientos
            bool solapa = await _reservaRepo.ExisteSolapamientoAprobadoAsync(
                reserva.EspacioId,
                reserva.FechaInicio,
                reserva.FechaFin,
                reserva.Id
            );

            if (solapa)
                return "Ya existe una reserva aprobada en este espacio para el rango de fechas indicado.";

            //cambio estado y guardar
            reserva.Estado = EstadoAprobada;
            await _reservaRepo.UpdateAsync(reserva);
            return null;
        }

        public async Task<string?> RechazarAsync(int reservaId)
        {
            // buscar reserva para rechazar
            var reserva = await _reservaRepo.GetByIdAsync(reservaId);
            if (reserva == null)
                return "La reserva no existe.";

            // solo rechazar reservas pendientes
            if (reserva.Estado != EstadoPendiente)
                return "Solo se pueden rechazar reservas pendientes.";

            // marcar como rechazada y guardar
            reserva.Estado = EstadoRechazada;
            await _reservaRepo.UpdateAsync(reserva);
            return null;
        }

        public async Task<string?> CancelarAsync(int reservaId, int usuarioId, bool esAdmin)
        {
            // buscar reserva para cancelar
            var reserva = await _reservaRepo.GetByIdAsync(reservaId);
            if (reserva == null)
                return "La reserva no existe.";

            // Si no es admin solo puede cancelar las reservas del propio usuario
            if (!esAdmin && reserva.UsuarioId != usuarioId)
                return "No puedes cancelar una reserva que no es tuya.";

            // Si ya esta cancelada o rechazada no se puede cancelar o rechazar
            if (reserva.Estado == EstadoCancelada || reserva.Estado == EstadoRechazada)
                return "Esta reserva ya no se puede cancelar.";

            // ambiar estado cancelado
            reserva.Estado = EstadoCancelada;
            await _reservaRepo.UpdateAsync(reserva);
            return null;
        }

        public async Task DeleteAsync(Reserva reserva)
        {
            // borrar reserva
            await _reservaRepo.DeleteAsync(reserva);
        }
    }
}
