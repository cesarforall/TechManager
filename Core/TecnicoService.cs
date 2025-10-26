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

        public async Task<(bool success, string message)> delete(int id)
        {
            if (id <= 0)
            {
                return (false, "ID de técnico inválido.");
            }
            try
            {
                bool result = await _tecnicoRepository.delete(id);

                if (result)
                {
                    return (true, "Técnico eliminado correctamente.");
                }
                return (false, $"No se encontró el técnico con el ID: {id}.");
            }
            catch (Exception)
            {
                return (false, "Error al eliminar el técnico.");
            }
        }

        public Task<(bool success, string message, List<Tecnico>?)> getAll()
        {
            throw new NotImplementedException();
        }

        public Task<(bool success, string message, Tecnico?)> getByGaveta(int gaveta)
        {
            throw new NotImplementedException();
        }

        public Task<(bool success, string message, Tecnico?)> getById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<(bool success, string message)> update(Tecnico tecnico)
        {
            if (tecnico.Id <= 0)
            {
                return (false, "El ID del técnico no es válido.");
            }
            if (string.IsNullOrWhiteSpace(tecnico.Nombre))
            {
                return (false, "El nombre del técnico es obligatorio.");
            }
            if (string.IsNullOrWhiteSpace(tecnico.Apellidos))
            {
                return (false, "Los apellidos del técnico son obligatorios.");
            }
            if (tecnico.Gaveta != null && tecnico.Gaveta > 0)
            {
                try
                {
                    var tecnicoByGaveta = await _tecnicoRepository.getByGaveta(tecnico.Gaveta.Value);
                    if (tecnicoByGaveta != null && tecnicoByGaveta.Id != tecnico.Id)
                    {
                        return (false, "La gaveta del técnico ya está asignada otro técnico.");
                    }
                }
                catch (Exception)
                {
                    return (false, "Error al comprobar la gaveta.");
                }
            }
            if (!string.IsNullOrWhiteSpace(tecnico.NombrePC))
            {
                try
                {
                    var tecnicoByNombrePC = await _tecnicoRepository.getByNombrePC(tecnico.NombrePC);
                    if (tecnicoByNombrePC != null && tecnicoByNombrePC.Id != tecnico.Id)
                    {
                        return (false, "El PC ya está asignado a otro técnico.");
                    }
                }
                catch (Exception)
                {
                    return (false, "Error al comprobar el nombre del PC.");
                }
            }
            try
            {
                var result = await _tecnicoRepository.update(tecnico);
                if (result)
                {
                    return (true, "Técnico actualizado correctamente.");
                }
                else
                {
                    return (false, $"No se encontró el técnico con el ID: {tecnico.Id}.");
                }
            }
            catch (Exception)
            {
                return (false, "Error al crear el técnico");
            }
        }
    }
}
