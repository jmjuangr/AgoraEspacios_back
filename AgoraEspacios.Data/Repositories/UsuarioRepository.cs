using AgoraEspacios.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgoraEspacios.Data.Repositories
{
    public class UsuarioRepository
    {
        private readonly EspaciosDbContext _context;

        public UsuarioRepository(EspaciosDbContext context)
        {
            _context = context;
        }

        // Obtener usuario por email
        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        // Obtener usuario por id
        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        // Crear usuario
        public async Task AddAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        // Actualizar usuario
        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        // Eliminar usuario
        public async Task DeleteAsync(Usuario usuario)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }

        // Listar todos 
        public async Task<List<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // Verificar si existe un usuario con un email dado
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }
    }
}
