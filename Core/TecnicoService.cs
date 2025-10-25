using Core.Interfaces;
using Core.Models;

namespace Core
{
    public class TecnicoService : ITecnicoService
    {
        private readonly ITecnicoRepository _tecnicoRepository;

        public TecnicoService(ITecnicoRepository tecnicoRepository)
        {
            _tecnicoRepository = tecnicoRepository;
        }

        public async Task<(bool success, string message, int id)> create(Tecnico tecnico)
        {
            if (string.IsNullOrWhiteSpace(tecnico.Nombre))
            {
                return (false, "El nombre del técnico es obligatorio.", 0);
            }
            if (string.IsNullOrWhiteSpace(tecnico.Apellidos))
            {
                return (false, "Los apellidos del técnico son obligatorios.", 0);
            }
            if (tecnico.Gaveta != null && tecnico.Gaveta > 0)
            {
                try
                {
                    var tecnicoByGaveta = await _tecnicoRepository.getByGaveta(tecnico.Gaveta.Value);
                    if (tecnicoByGaveta != null)
                    {
                        return (false, "La gaveta del técnico ya está asignada a otro técnico.", 0);
                    }
                }
                catch (Exception)
                {
                    return (false, "Error al comprobar la gaveta.", 0);
                }
            }
            if (!string.IsNullOrWhiteSpace(tecnico.NombrePC))
            {
                try
                {
                    var tecnicoByNombrePC = await _tecnicoRepository.getByNombrePC(tecnico.NombrePC);
                    if (tecnicoByNombrePC != null)
                    {
                        return (false, "El PC ya está asignado a otro técnico.", 0);
                    }
                }
                catch (Exception)
                {
                    return (false, "Error al comprobar el nombre del PC.", 0);
                }
            }
            try
            {
                int id = await _tecnicoRepository.create(tecnico);
                return (true, $"Técnico creado correctamente con ID {id}.", id);
            }
            catch (Exception)
            {
                return (false, "Error al crear el técnico", 0);
            } 
        }

        public Task<(bool success, string message)> delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Tecnico>> getAll()
        {
            throw new NotImplementedException();
        }

        public Task<Tecnico?> getByGaveta(int gaveta)
        {
            throw new NotImplementedException();
        }

        public Task<Tecnico?> getById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<(bool success, string message)> update(Tecnico tecnico)
        {
            throw new NotImplementedException();
        }
    }
}
