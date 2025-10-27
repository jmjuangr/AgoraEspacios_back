using AgoraEspacios.Data.Repositories;
using AgoraEspacios.Models.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AgoraEspacios.Business.Services
{
    public class UsuarioService
    {
        private readonly UsuarioRepository _usuarioRepository;

        public UsuarioService(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _usuarioRepository.GetByEmailAsync(email);
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _usuarioRepository.GetByIdAsync(id);
        }

        public async Task<List<Usuario>> GetAllAsync()
        {
            return await _usuarioRepository.GetAllAsync();
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            await _usuarioRepository.AddAsync(usuario);
            return usuario;
        }
    }
}
