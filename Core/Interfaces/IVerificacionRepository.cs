using Core.Models;

namespace Core.Interfaces
{
    public interface IVerificacionRepository
    {
        Task<List<Verificacion>> GetAll();
        Task<Verificacion> GetById(int id);
        Task<List<Verificacion>> GetByActualizacionId(int actualizacionId);
        Task<int> Create(Verificacion verificacion);
        Task<bool> ConfirmVerification(int actualizacionId, int tecnicoId, string fechaConfirmacion);
    }
}
