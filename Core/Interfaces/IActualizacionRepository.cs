using Core.Models;

namespace Core.Interfaces
{
    public interface IActualizacionRepository
    {
        Task<List<Actualizacion>> GetAll();
        Task<Actualizacion> GetById(int id);
        Task<List<Actualizacion>> GetByDispositivoId(int dispositivoId);
        Task<int> Create(Actualizacion update);
        Task<bool> Delete(int id);
    }
}
