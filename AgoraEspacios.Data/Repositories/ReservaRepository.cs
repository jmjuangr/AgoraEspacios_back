using AgoraEspacios.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgoraEspacios.Data.Repositories
{
    public class ReservaRepository
    {
        private readonly EspaciosDbContext _context;

        public ReservaRepository(EspaciosDbContext context)
        {
            _context = context;
        }


        public async Task<List<Reserva>> GetAllAsync()
        {
            return await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Espacio)
                .ToListAsync();
        }


        public async Task<List<Reserva>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Reservas
                .Include(r => r.Espacio)
                .Where(r => r.UsuarioId == usuarioId)
                .ToListAsync();
        }


        public async Task<Reserva?> GetByIdAsync(int id)
        {
            return await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Espacio)
                .FirstOrDefaultAsync(r => r.Id == id);
        }


        public async Task AddAsync(Reserva reserva)
        {
            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(Reserva reserva)
        {
            _context.Reservas.Update(reserva);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync(Reserva reserva)
        {
            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();
        }

        // Buscar reservas solapadas
        public async Task<bool> ExisteSolapamientoAsync(int espacioId, DateTime inicio, DateTime fin, int? reservaId = null)
        {
            return await _context.Reservas.AnyAsync(r =>
                r.EspacioId == espacioId &&
                (reservaId == null || r.Id != reservaId) &&
                r.FechaInicio < fin &&
                r.FechaFin > inicio
            );
        }
    }
}
