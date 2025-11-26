using Core.Interfaces;
using Core.Models;

namespace Core.Services
{
    public class VerificacionService : IVerificacionService
    {
        private readonly IVerificacionRepository _verificacionRepository;

        public event EventHandler<int>? VerificacionCreated;
        public event EventHandler<int>? VerificacionConfirmed;

        public VerificacionService(IVerificacionRepository verificacionRepository)
        {
            _verificacionRepository = verificacionRepository;
        }

        public async Task<(bool success, string message, int id)> Create(Verificacion verificacion)
        {
            try
            {
                if (verificacion == null)
                {
                    return (false, "Los datos de la verificación no pueden estar vacíos.", 0);
                }

                if (verificacion.ActualizacionId <= 0)
                {
                    return (false, "El ID de la actualización es obligatorio.", 0);
                }

                if (verificacion.TecnicoId <= 0)
                {
                    return (false, "El ID del técnico es obligatorio.", 0);
                }

                var verificacionId = await _verificacionRepository.Create(verificacion);

                if (verificacionId > 0)
                {
                    VerificacionCreated?.Invoke(this, verificacionId);
                    return (true, "Verificación asignada correctamente.", verificacionId);
                }
                else
                {
                    return (false, "Error al asignar la verificación.", 0);
                }
            }
            catch (Exception)
            {
                return (false, "Error al asignar la verificación.", 0);
            }
        }

        public async Task<(bool success, string message)> ConfirmVerification(int actualizacionId, int tecnicoId)
        {
            if (actualizacionId <= 0)
            {
                return (false, "ID de actualización inválido.");
            }

            if (tecnicoId <= 0)
            {
                return (false, "ID de técnico inválido.");
            }

            try
            {
                var fechaConfirmacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result = await _verificacionRepository.ConfirmVerification(actualizacionId, tecnicoId, fechaConfirmacion);

                if (result)
                {
                    VerificacionConfirmed?.Invoke(this, actualizacionId);
                    return (true, "Verificación confirmada correctamente.");
                }
                else
                {
                    return (false, $"No se encontró la verificación para actualización {actualizacionId} y técnico {tecnicoId}.");
                }
            }
            catch (Exception)
            {
                return (false, "Error al confirmar la verificación.");
            }
        }

        public async Task<(bool success, string message, List<Verificacion> verificaciones)> GetAll()
        {
            try
            {
                var result = await _verificacionRepository.GetAll();

                return (result.Count > 0)
                    ? (true, "Verificaciones obtenidas correctamente.", result)
                    : (true, "No existen registros de verificaciones.", result);
            }
            catch (Exception)
            {
                return (false, "Error al obtener las verificaciones.", new List<Verificacion>());
            }
        }

        public async Task<(bool success, string message, Verificacion verificacion)> GetById(int id)
        {
            try
            {
                var result = await _verificacionRepository.GetById(id);

                return (result.Id > 0)
                    ? (true, "Verificación obtenida correctamente.", result)
                    : (true, $"No existe la verificación con ID: {id}.", result);
            }
            catch (Exception)
            {
                return (false, "Error al obtener la verificación.", new Verificacion());
            }
        }

        public async Task<(bool success, string message, List<Verificacion> verificaciones)> GetByActualizacionId(int actualizacionId)
        {
            if (actualizacionId <= 0)
            {
                return (false, "ID de actualización inválido.", new List<Verificacion>());
            }

            try
            {
                var result = await _verificacionRepository.GetByActualizacionId(actualizacionId);

                return (result.Count > 0)
                    ? (true, "Verificaciones obtenidas correctamente.", result)
                    : (true, $"No existen verificaciones para la actualización con ID: {actualizacionId}.", result);
            }
            catch (Exception)
            {
                return (false, "Error al obtener las verificaciones por actualización.", new List<Verificacion>());
            }
        }
    }
}
