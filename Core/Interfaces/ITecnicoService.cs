using Core.Models;

namespace Core.Interfaces
{
    public interface ITecnicoService
    {
        Task<(bool success, string message, int id)> create(Tecnico tecnico);
        Task<(bool success, string message)> delete(int id);
        Task<List<Tecnico>> getAll();
        Task<Tecnico?> getById(int id);
        Task<(bool success, string message)> update(Tecnico tecnico);
        Task<Tecnico?> getByGaveta(int gaveta);
    }
}
