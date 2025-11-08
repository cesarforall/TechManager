using Core.Models;

namespace Core.Interfaces
{
    public interface IDispositivoService
    {
        Task<(bool success, string message, int? id)> Create(Dispositivo dispositivo);
        Task<(bool success, string message)> Delete(int id);
        Task<(bool success, string message, List<Dispositivo>? dispositivo)> GetAll();
        Task<(bool success, string message, Dispositivo? dispositivo)> GetById(int id);
        Task<(bool success, string message)> Update(Dispositivo dispositivo);
    }
}
