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
            // Busco un usuario por email
            return await _usuarioRepository.GetByEmailAsync(email);
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            // buscar un usuario por id
            return await _usuarioRepository.GetByIdAsync(id);
        }

        public async Task<List<Usuario>> GetAllAsync()
        {
            //devolver lista completa usuarios
            return await _usuarioRepository.GetAllAsync();
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            // guardar nuevo usuario
            await _usuarioRepository.AddAsync(usuario);
            return usuario;
        }

        public async Task<(bool Success, string Message)> UpdateAsync(Usuario usuario)
        {
            // primero busco el usuario guardado para saber si existe
            var existing = await _usuarioRepository.GetByIdAsync(usuario.Id);
            if (existing == null)
                return (false, "El usuario no existe.");

            // si cambia el email, reviso que no lo tenga otro usuario
            if (existing.Email != usuario.Email && await _usuarioRepository.ExistsByEmailAsync(usuario.Email))
                return (false, "Ya existe otro usuario con este email.");

            // actualizo solo los datos editables y dejo la contraseña como estaba
            existing.Nombre = usuario.Nombre;
            existing.Email = usuario.Email;
            existing.Rol = usuario.Rol;

            await _usuarioRepository.UpdateAsync(existing);
            return (true, "Usuario actualizado correctamente.");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            // antes de borrar compruebo que el usuario exista
            var existing = await _usuarioRepository.GetByIdAsync(id);
            if (existing == null)
                return (false, "El usuario no existe.");

            // si existe, se elimina desde el repositorio
            await _usuarioRepository.DeleteAsync(existing);
            return (true, "Usuario eliminado correctamente.");
        }
    }
}
