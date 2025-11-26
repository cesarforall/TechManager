using Core.Interfaces;
using Core.Models;

namespace Core.Services
{
    public class DispositivoService : IDispositivoService
    {
        private readonly IDispositivoRepository _dispositivoRepository;

        public event EventHandler<int>? DispositivoCreated;
        public event EventHandler<int>? DispositivoUpdated;

        public DispositivoService(IDispositivoRepository dispositivoRepository)
        {
            _dispositivoRepository = dispositivoRepository;
        }

        public async Task<(bool success, string message, int? id)> Create(Dispositivo dispositivo)
        {

            if (dispositivo == null)
            {
                return (false, $"Los datos del técnico no pueden estar vacios", null);
            }
            if (string.IsNullOrWhiteSpace(dispositivo.Fabricante))
            {
                return (false, $"El campo fabricante es obligatorio", null);                
            }
            if (string.IsNullOrWhiteSpace(dispositivo.Modelo))
            {
                return (false, $"El campo modelo es obligatorio", null);                
            }
            try
            {
                var result = await _dispositivoRepository.GetByFabricanteModelo(dispositivo.Fabricante, dispositivo.Modelo);

                if (result != null)
                {
                    return (false, $"El dispositivo ya está registrado en la base de datos.", null);
                }
            }
            catch (Exception)
            {
                return (false, $"Error al crear el dispositivo.", null);
            }
            try
            {
                var newId = await _dispositivoRepository.Create(dispositivo);

                if (newId == null)
                {
                    return (false, $"Error al crear el dispositivo.", null);
                }

                if (newId > 0)
                {
                    DispositivoCreated?.Invoke(this, newId.Value);
                }

                return (true, $"Dispositivo creado correctamente con ID: {newId}", newId);
            }
            catch (Exception)
            {
                return (false, $"Error al crear el dispositivo.", null);
            }
        }

        public async Task<(bool success, string message)> Delete(int id)
        {
            if (id <= 0)
            {
                return (false, "ID de dispositivo inválido.");
            }
            try
            {
                bool result = await _dispositivoRepository.Delete(id);

                return result
                    ? (true, "Dispositivo eliminado correctamente.")
                    : (false, $"No se encontró el técnico con ID: {id}");
                
            }
            catch (Exception)
            {
                return (false, "Error al eliminar el dispositivo.");
            }
        }

        public async Task<(bool success, string message, List<Dispositivo>? dispositivo)> GetAll()
        {
            try
            {
                var result = await _dispositivoRepository.GetAll();

                return (result == null)
                    ? (false, "No existen dispositivos en la base de datos.", null)
                    : (true, "Dispositivos obtenidos correctamente.", result);
            }
            catch (Exception)
            {
                return (false, "Error al obtener los dispositivos.", null);
            }
        }

        public async Task<(bool success, string message, Dispositivo? dispositivo)> GetById(int id)
        {
            if (id <= 0)
            {
                return (false, $"ID de dispositivo inválido.", null);
            }
            try
            {
                var result = await _dispositivoRepository.GetById(id);

                return (result == null)
                    ? (false, $"No se encontró un dispositivo con id {id}.", null)
                    : (true, "Dispositivo obtenido correctamente.", result);
            }
            catch (Exception)
            {
                return (false, $"Error al obtener el dispositivo por id.", null);
            }
        }

        public async Task<(bool success, string message)> Update(Dispositivo dispositivo)
        {
            if (dispositivo.Id <= 0 || dispositivo.Id == null) return (false, $"ID del dispositivo inválido.");
            if (string.IsNullOrWhiteSpace(dispositivo.Fabricante)) return (false, $"El campo fabricante es obligatorio.");
            if (string.IsNullOrWhiteSpace(dispositivo.Modelo)) return (false, $"El campo modelo es obligatorio.");
            try
            {
                var dFabricanteModelo = await _dispositivoRepository.GetByFabricanteModelo(dispositivo.Fabricante, dispositivo.Modelo);

                if (dFabricanteModelo != null && dFabricanteModelo.Id != dispositivo.Id)
                {
                    return (false, $"El dispositivo ya existe en la base de datos.");
                }
                try
                {
                    bool result = await _dispositivoRepository.Update(dispositivo);

                    if (result && dispositivo.Id.HasValue)
                    {
                        DispositivoUpdated?.Invoke(this, dispositivo.Id.Value);
                        return (true, "Dispositivo actualizado correctamente.");
                    }
                    else
                    {
                        return (false, $"No se encontró el técnico con el ID: {dispositivo.Id}");
                    }
                }
                catch (Exception)
                {
                    return (false, "Error al actualizar el dispositivo.");
                }
            }
            catch (Exception)
            {
                return (false, "Error al comprobar el dispositivo por fabricante y modelo.");
            }
        }
    }
}
