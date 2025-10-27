using AgoraEspacios.Data.Repositories;
using AgoraEspacios.Models.Entities;

namespace AgoraEspacios.Business.Services
{
    public class EspacioService
    {
        private readonly EspacioRepository _repository;

        public EspacioService(EspacioRepository repository)
        {
            _repository = repository;
        }

        // Obtener todos los espacios
        public async Task<List<Espacio>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        // Obtener un espacio por Id
        public async Task<Espacio?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        // Crear un nuevo espacio con validaciones
        public async Task<(bool Success, string Message)> CreateAsync(Espacio espacio)
        {
            // Validar nombre
            if (string.IsNullOrWhiteSpace(espacio.Nombre))
                return (false, "El nombre del espacio es obligatorio.");

            if (espacio.Nombre.Length > 100)
                return (false, "El nombre del espacio no puede superar los 100 caracteres.");

            // Validar capacidad
            if (espacio.Capacidad <= 0)
                return (false, "La capacidad debe ser mayor que cero.");

            // Validar categoría existente
            if (!await _repository.CategoriaExistsAsync(espacio.CategoriaEspacioId))
                return (false, "La categoría asociada no existe.");

            // Validar duplicados
            if (await _repository.ExistsByNameAsync(espacio.Nombre))
                return (false, "Ya existe un espacio con este nombre.");

            await _repository.AddAsync(espacio);
            return (true, "Espacio creado correctamente.");
        }

        // Actualizar espacio
        public async Task<(bool Success, string Message)> UpdateAsync(Espacio espacio)
        {
            var existing = await _repository.GetByIdAsync(espacio.Id);
            if (existing == null)
                return (false, "El espacio no existe.");

            // Validar duplicados (si cambia el nombre)
            if (existing.Nombre != espacio.Nombre && await _repository.ExistsByNameAsync(espacio.Nombre))
                return (false, "Ya existe otro espacio con este nombre.");

            existing.Nombre = espacio.Nombre;
            existing.Capacidad = espacio.Capacidad;
            existing.Ubicacion = espacio.Ubicacion;
            existing.Descripcion = espacio.Descripcion;
            existing.ImagenUrl = espacio.ImagenUrl;
            existing.CategoriaEspacioId = espacio.CategoriaEspacioId;

            await _repository.UpdateAsync(existing);

            return (true, "Espacio actualizado correctamente.");
        }

        // Eliminar espacio
        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return (false, "El espacio no existe.");

            await _repository.DeleteAsync(existing);
            return (true, "Espacio eliminado correctamente.");
        }
    }
}
