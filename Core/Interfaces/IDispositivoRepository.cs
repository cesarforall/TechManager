using Core.Models;

namespace Core.Interfaces
{
    public interface IDispositivoRepository
    {
        Task<int> Create(Dispositivo dispositivo);
        Task<bool> Delete(int id);
        Task<List<Dispositivo>?> GetAll();
        Task<Dispositivo?> GetById(int id);
        Task<bool> Update(Dispositivo dispositivo);
    }
}
