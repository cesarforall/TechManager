using Core.Models;

namespace Core.Interfaces
{
    public interface ITecnicoRepository
    {
        Task<int> create(Tecnico tecnico);
        Task<bool> delete(int id);
        Task<List<Tecnico>> getAll();
        Task<Tecnico?> getById(int id);
        Task<bool> update(Tecnico tecnico);
        Task<Tecnico?> getByGaveta(int gaveta);
        Task<Tecnico?> getByNombrePC(string nombrePC);
    }
}
