using Core.Interfaces;
using Core.Models;

namespace Core.Services
{
    public class ConocimientoService : IConocimientoService
    {
        private readonly IConocimientoRepository _conocimientoRepository;
        public ConocimientoService(IConocimientoRepository conocimientoRepository)
        {
            _conocimientoRepository = conocimientoRepository;
        }
        public async Task<(bool success, string message, int id)> Create(Conocimiento conocimiento)
        {
            try
            {
                var result = await _conocimientoRepository.Create(conocimiento);

                return result > 0
                    ? (true, "Conocimiento asignado correctamente.", result)
                    : (false, "Error al asignar el conocimiento.", result);
            }
            catch (Exception)
            {
                return (false, "Error al asignar el conocimiento.", 0);
            }
        }

        public async Task<(bool success, string message)> Delete(int id)
        {
            if (id <= 0) return (false, "ID de conocimiento inválido.");
            try
            {
                var result = await _conocimientoRepository.Delete(id);

                return result
                    ? (true, "Conocimiento desasignado correctamente.")
                    : (false, $"No se encontró el conocimiento con ID: {id}.");
            }
            catch (Exception)
            {
                return (false, "Error al desasignar el conocimiento.");
            }
        }

        public async Task<(bool success, string message, List<Conocimiento> conocimientos)> GetAll()
        {
            try
            {
                var result = await _conocimientoRepository.GetAll();

                return (result.Count > 0)
                    ? (true, "Conocimientos obtenidos correctamente.", result)
                    : (true, "No existen registros de conocimientos.", result);

            }
            catch (Exception)
            {
                return (false, "Error al obtener los conocimientos.", new List<Conocimiento>());
            }
        }
    }
}
