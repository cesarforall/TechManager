using Core.Models;

namespace Core.Interfaces
{
    public interface IConocimientoService
    {
        event EventHandler<int>? ConocimientoCreated;
        Task<(bool success, string message, List<Conocimiento> conocimientos)> GetAll();
        Task<(bool success, string message, Conocimiento conocimiento)> GetById(int id);
        Task<(bool success, string message)> Delete(int id);
        Task<(bool success, string message, int id)> Create(Conocimiento conocimiento);
        Task<(bool success, string message, List<Conocimiento> conocimientos)> GetAvailableConocimientosByTecnicoId(int tecnicoId);
    }
}
