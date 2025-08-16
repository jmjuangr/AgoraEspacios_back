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

        // Listar todos 
        public async Task<List<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }
    }
}
