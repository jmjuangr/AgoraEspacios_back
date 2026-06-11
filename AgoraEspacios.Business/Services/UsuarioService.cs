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

        public async Task<bool> ExistsByNifAsync(string nif)
        {
            // Comparo el NIF ya limpio para evitar diferencias por espacios o minusculas
            return await _usuarioRepository.ExistsByNifAsync(NormalizeNif(nif));
        }

        public async Task<List<Usuario>> GetAllAsync()
        {
            //devolver lista completa usuarios
            return await _usuarioRepository.GetAllAsync();
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            // guardar nif
            usuario.Nif = NormalizeNif(usuario.Nif);

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

            // Limpio el NIF antes de comprobarlo y guardarlo
            usuario.Nif = NormalizeNif(usuario.Nif);

            // si cambia el NIF, reviso que no lo tenga otro usuario
            if (existing.Nif != usuario.Nif && await _usuarioRepository.ExistsByNifAsync(usuario.Nif))
                return (false, "Ya existe otro usuario con este NIF.");

            // actualizo solo los datos editables y dejo la contrasena como estaba
            existing.Nombre = usuario.Nombre;
            existing.Email = usuario.Email;
            existing.Nif = usuario.Nif;
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

        private static string NormalizeNif(string nif)
        {
            // Quito espacios y pongo letras en mayuscula
            return nif.Trim().ToUpperInvariant();
        }
    }
}
