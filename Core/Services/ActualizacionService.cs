using Core.Interfaces;
using Core.Models;

namespace Core.Services
{
    public class ActualizacionService : IActualizacionService
    {
        private readonly IActualizacionRepository _actualizacionRepository;
        private readonly IDispositivoService _dispositivoService;

        public event EventHandler<int>? ActualizacionCreated;

        public ActualizacionService(IActualizacionRepository actualizacionRepository, IDispositivoService dispositivoService)
        {
            _actualizacionRepository = actualizacionRepository;
            _dispositivoService = dispositivoService;
        }

        public async Task<(bool success, string message, int? id)> Create(Actualizacion actualizacion)
        {
            try
            {
                if (actualizacion == null)
                {
                    return (false, "Los datos de la actualización no pueden estar vacíos.", null);
                }

                if (actualizacion.DispositivoId <= 0)
                {
                    return (false, "El ID del dispositivo es obligatorio.", null);
                }

                if (string.IsNullOrWhiteSpace(actualizacion.Version))
                {
                    return (false, "El campo versión es obligatorio.", null);
                }

                if (string.IsNullOrWhiteSpace(actualizacion.Descripcion))
                {
                    return (false, "El campo descripción es obligatorio.", null);
                }

                if (string.IsNullOrWhiteSpace(actualizacion.Fecha))
                {
                    return (false, "El campo fecha es obligatorio.", null);
                }

                // Validar y normalizar formato
                if (!DateTime.TryParse(actualizacion.Fecha, out DateTime fecha))
                {
                    return (false, "Formato de fecha inválido.", null);
                }

                // Normalizar a formato ISO con hora
                actualizacion.Fecha = fecha.ToString("yyyy-MM-dd HH:mm:ss");

                var dispositivoResult = await _dispositivoService.GetById(actualizacion.DispositivoId);
                if (!dispositivoResult.success || dispositivoResult.dispositivo == null)
                {
                    return (false, $"No existe un dispositivo con ID: {actualizacion.DispositivoId}.", null);
                }

                var actualizacionesExistentes = await _actualizacionRepository.GetByDispositivoId(actualizacion.DispositivoId);
                if (actualizacionesExistentes.Any(a => a.Version == actualizacion.Version))
                {
                    return (false, $"Ya existe una actualización con la versión {actualizacion.Version} para este dispositivo.", null);
                }

                var actualizacionId = await _actualizacionRepository.Create(actualizacion);

                if (actualizacionId > 0)
                {
                    ActualizacionCreated?.Invoke(this, actualizacionId);
                    return (true, $"Actualización creada correctamente con ID: {actualizacionId}.", actualizacionId);
                }
                else
                {
                    return (false, "Error al crear la actualización.", null);
                }
            }
            catch (Exception)
            {
                return (false, "Error al crear la actualización.", null);
            }
        }

        public async Task<(bool success, string message)> Delete(int id)
        {
            if (id <= 0)
            {
                return (false, "ID de actualización inválido.");
            }

            try
            {
                var result = await _actualizacionRepository.Delete(id);

                return result
                    ? (true, "Actualización eliminada correctamente.")
                    : (false, $"No se encontró la actualización con ID: {id}.");
            }
            catch (Exception)
            {
                return (false, "Error al eliminar la actualización.");
            }
        }

        public async Task<(bool success, string message, List<Actualizacion>? actualizaciones)> GetAll()
        {
            try
            {
                var result = await _actualizacionRepository.GetAll();

                return (result.Count > 0)
                    ? (true, "Actualizaciones obtenidas correctamente.", result)
                    : (false, "No existen actualizaciones en la base de datos.", null);
            }
            catch (Exception)
            {
                return (false, "Error al obtener las actualizaciones.", null);
            }
        }

        public async Task<(bool success, string message, Actualizacion? actualizacion)> GetById(int id)
        {
            if (id <= 0)
            {
                return (false, "ID de actualización inválido.", null);
            }

            try
            {
                var result = await _actualizacionRepository.GetById(id);

                return result != null && result.Id > 0
                    ? (true, "Actualización obtenida correctamente.", result)
                    : (false, $"No se encontró la actualización con ID: {id}.", null);
            }
            catch (Exception)
            {
                return (false, "Error al obtener la actualización.", null);
            }
        }

        public async Task<(bool success, string message, List<Actualizacion>? actualizaciones)> GetByDispositivoId(int dispositivoId)
        {
            if (dispositivoId <= 0)
            {
                return (false, "ID de dispositivo inválido.", null);
            }

            try
            {
                var result = await _actualizacionRepository.GetByDispositivoId(dispositivoId);

                return (result.Count > 0)
                    ? (true, "Actualizaciones obtenidas correctamente.", result)
                    : (false, $"No existen actualizaciones para el dispositivo con ID: {dispositivoId}.", null);
            }
            catch (Exception)
            {
                return (false, "Error al obtener las actualizaciones por dispositivo.", null);
            }
        }
    }
}
