using Core.Models;

namespace Core.Interfaces
{
    public interface ITecnicoService
    {
        Task<(bool success, string message, int id)> create(Tecnico tecnico);
        Task<(bool success, string message)> delete(int id);
        Task<(bool success, string message, List<Tecnico>? tecnicos)> getAll();
        Task<(bool success, string message, Tecnico? tecnico)> getById(int id);
        Task<(bool success, string message)> update(Tecnico tecnico);
        Task<(bool success, string message, Tecnico? tecnico)> getByGaveta(int gaveta);
    }
}
