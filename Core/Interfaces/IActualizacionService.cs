using Core.Models;

namespace Core.Interfaces
{
    public interface IActualizacionService
    {
        public event EventHandler<int>? ActualizacionCreated;
        Task<(bool success, string message, int? id)> Create(Actualizacion actualizacion);
        Task<(bool success, string message)> Delete(int id);
        Task<(bool success, string message, List<Actualizacion>? actualizaciones)> GetAll();
        Task<(bool success, string message, Actualizacion? actualizacion)> GetById(int id);
        Task<(bool success, string message, List<Actualizacion>? actualizaciones)> GetByDispositivoId(int dispositivoId);
    }
}
