using AgoraEspacios.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgoraEspacios.Data.Repositories
{
    public class CategoriaEspacioRepository
    {
        private readonly EspaciosDbContext _context;

        public CategoriaEspacioRepository(EspaciosDbContext context)
        {
            _context = context;
        }

        // Obtener todas las categorías
        public async Task<List<CategoriaEspacio>> GetAllAsync()
        {
            return await _context.CategoriaEspacios.ToListAsync();
        }

        // Obtener una categoría por Id
        public async Task<CategoriaEspacio?> GetByIdAsync(int id)
        {
            return await _context.CategoriaEspacios.FirstOrDefaultAsync(c => c.Id == id);
        }

        // Crear nueva categoría
        public async Task AddAsync(CategoriaEspacio categoria)
        {
            _context.CategoriaEspacios.Add(categoria);
            await _context.SaveChangesAsync();
        }

        // Actualizar categoría existente
        public async Task UpdateAsync(CategoriaEspacio categoria)
        {
            _context.CategoriaEspacios.Update(categoria);
            await _context.SaveChangesAsync();
        }

        // Eliminar categoría
        public async Task DeleteAsync(CategoriaEspacio categoria)
        {
            _context.CategoriaEspacios.Remove(categoria);
            await _context.SaveChangesAsync();
        }

        // Verificar si existe una categoría con un nombre dado
        public async Task<bool> ExistsByNameAsync(string nombre)
        {
            return await _context.CategoriaEspacios.AnyAsync(c => c.Nombre == nombre);
        }
    }
}
