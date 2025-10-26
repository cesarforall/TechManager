using Core.Models;

namespace Core.Interfaces
{
    public interface ITecnicoService
    {
        Task<(bool success, string message, int id)> create(Tecnico tecnico);
        Task<(bool success, string message)> delete(int id);
        Task<(bool success, string message, List<Tecnico>?)> getAll();
        Task<(bool success, string message, Tecnico?)> getById(int id);
        Task<(bool success, string message)> update(Tecnico tecnico);
        Task<(bool success, string message, Tecnico?)> getByGaveta(int gaveta);
    }
}
