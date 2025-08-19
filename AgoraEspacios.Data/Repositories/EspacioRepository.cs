using AgoraEspacios.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgoraEspacios.Data.Repositories
{
    public class EspacioRepository
    {
        private readonly EspaciosDbContext _context;

        public EspacioRepository(EspaciosDbContext context)
        {
            _context = context;
        }

        // Obtener todos los espacios (incluyendo la categoría asociada)
        public async Task<List<Espacio>> GetAllAsync()
        {
            return await _context.Espacios
                                 .Include(e => e.Categoria)
                                 .ToListAsync();
        }

        // Obtener un espacio por Id (con categoría)
        public async Task<Espacio?> GetByIdAsync(int id)
        {
            return await _context.Espacios
                                 .Include(e => e.Categoria)
                                 .FirstOrDefaultAsync(e => e.Id == id);
        }

        // Crear un nuevo espacio
        public async Task AddAsync(Espacio espacio)
        {
            _context.Espacios.Add(espacio);
            await _context.SaveChangesAsync();
        }

        // Actualizar un espacio existente
        public async Task UpdateAsync(Espacio espacio)
        {
            _context.Espacios.Update(espacio);
            await _context.SaveChangesAsync();
        }

        // Eliminar un espacio
        public async Task DeleteAsync(Espacio espacio)
        {
            _context.Espacios.Remove(espacio);
            await _context.SaveChangesAsync();
        }

        // Verificar si existe un espacio con un nombre dado
        public async Task<bool> ExistsByNameAsync(string nombre)
        {
            return await _context.Espacios.AnyAsync(e => e.Nombre == nombre);
        }

        // Verificar si existe una categoría asociada
        public async Task<bool> CategoriaExistsAsync(int categoriaId)
        {
            return await _context.CategoriaEspacios.AnyAsync(c => c.Id == categoriaId);
        }
    }
}
