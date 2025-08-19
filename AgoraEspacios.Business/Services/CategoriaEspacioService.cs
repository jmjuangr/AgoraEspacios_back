using AgoraEspacios.Data.Repositories;
using AgoraEspacios.Models.Entities;

namespace AgoraEspacios.Business.Services
{
    public class CategoriaEspacioService
    {
        private readonly CategoriaEspacioRepository _repository;

        public CategoriaEspacioService(CategoriaEspacioRepository repository)
        {
            _repository = repository;
        }

        // Obtener todas las categorías
        public async Task<List<CategoriaEspacio>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        // Obtener categoría por Id
        public async Task<CategoriaEspacio?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        // Crear nueva categoría con validación de nombre único
        public async Task<(bool Success, string Message)> CreateAsync(CategoriaEspacio categoria)
        {
            // Validar longitud
            if (string.IsNullOrWhiteSpace(categoria.Nombre) || categoria.Nombre.Length < 3)
                return (false, "El nombre de la categoría debe tener al menos 3 caracteres.");

            if (categoria.Nombre.Length > 100)
                return (false, "El nombre de la categoría no puede superar los 100 caracteres.");

            // Validar duplicados
            if (await _repository.ExistsByNameAsync(categoria.Nombre))
                return (false, "Ya existe una categoría con este nombre.");

            await _repository.AddAsync(categoria);
            return (true, "Categoría creada correctamente.");
        }

        // Actualizar categoría
        public async Task<(bool Success, string Message)> UpdateAsync(CategoriaEspacio categoria)
        {
            var existing = await _repository.GetByIdAsync(categoria.Id);
            if (existing == null)
                return (false, "La categoría no existe.");

            // Validar duplicados (si el nombre cambió)
            if (existing.Nombre != categoria.Nombre && await _repository.ExistsByNameAsync(categoria.Nombre))
                return (false, "Ya existe otra categoría con este nombre.");

            existing.Nombre = categoria.Nombre;
            await _repository.UpdateAsync(existing);

            return (true, "Categoría actualizada correctamente.");
        }

        // Eliminar categoría
        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return (false, "La categoría no existe.");

            await _repository.DeleteAsync(existing);
            return (true, "Categoría eliminada correctamente.");
        }
    }
}
