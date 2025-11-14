using Core.Models;

namespace Core.Interfaces
{
    public interface IConocimientoRepository
    {
        Task<List<Conocimiento>> GetAll();
        Task<Conocimiento> GetById(int id);
        Task<bool> Delete(int id);
        Task<int> Create(Conocimiento conocimiento);
    }
}
