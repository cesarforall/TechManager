using Core.Models;

namespace Core.Interfaces
{
    public interface IVerificacionService
    {
        Task<(bool success, string message, int id)> Create(Verificacion verificacion);
        Task<(bool success, string message)> ConfirmVerification(int actualizacionId, int tecnicoId);
        Task<(bool success, string message, List<Verificacion> verificaciones)> GetAll();
        Task<(bool success, string message, Verificacion verificacion)> GetById(int id);
        Task<(bool success, string message, List<Verificacion> verificaciones)> GetByActualizacionId(int actualizacionId);
    }
}
