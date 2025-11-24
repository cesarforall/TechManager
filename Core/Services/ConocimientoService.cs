using Core.Interfaces;
using Core.Models;

namespace Core.Services
{
    public class ConocimientoService : IConocimientoService
    {
        private readonly IConocimientoRepository _conocimientoRepository;
        private readonly IDispositivoService _dispositivoService;
        private readonly ITecnicoService _tecnicoService;

        public event EventHandler<int>? ConocimientoCreated;
        public ConocimientoService(IConocimientoRepository conocimientoRepository, IDispositivoService dispositivoService, ITecnicoService tecnicoService)
        {
            _conocimientoRepository = conocimientoRepository;
            _dispositivoService = dispositivoService;
            _tecnicoService = tecnicoService;
        }
        public async Task<(bool success, string message, int id)> Create(Conocimiento conocimiento)
        {
            try
            {
                var conocimientoId = await _conocimientoRepository.Create(conocimiento);

                if (conocimientoId > 0)
                {
                    ConocimientoCreated?.Invoke(this, conocimientoId);

                    return (true, "Conocimiento asignado correctamente.", conocimientoId);
                }
                else
                {
                    return (false, "Error al asignar el conocimiento.", conocimientoId);
                }
            }
            catch (Exception)
            {
                return (false, "Error al asignar el conocimiento.", 0);
            }
        }

        public async Task<(bool success, string message)> Delete(int id)
        {
            if (id <= 0) return (false, "ID de conocimiento inválido.");
            try
            {
                var result = await _conocimientoRepository.Delete(id);

                return result
                    ? (true, "Conocimiento desasignado correctamente.")
                    : (false, $"No se encontró el conocimiento con ID: {id}.");
            }
            catch (Exception)
            {
                return (false, "Error al desasignar el conocimiento.");
            }
        }

        public async Task<(bool success, string message, List<Conocimiento> conocimientos)> GetAll()
        {
            try
            {
                var result = await _conocimientoRepository.GetAll();

                return (result.Count > 0)
                    ? (true, "Conocimientos obtenidos correctamente.", result)
                    : (true, "No existen registros de conocimientos.", result);

            }
            catch (Exception)
            {
                return (false, "Error al obtener los conocimientos.", new List<Conocimiento>());
            }
        }

        public async Task<(bool success, string message, List<Conocimiento> conocimientos)> GetAvailableConocimientosByTecnicoId(int tecnicoId)
        {
            Tecnico tecnico = new();
            List<Dispositivo> dispositivos = new();
            List<Conocimiento> availableConocimientos = new();

            try
            {
                var tecnicoResult = await _tecnicoService.getById(tecnicoId);

                if (tecnicoResult.success && tecnicoResult.tecnico != null)
                {
                    tecnico = tecnicoResult.tecnico;

                    var dispositivosResult = await _dispositivoService.GetAll();

                    if (dispositivosResult.success && dispositivosResult.dispositivo != null)
                    {
                        foreach (var dispositivo in dispositivosResult.dispositivo)
                        {
                            dispositivos.Add(new Dispositivo
                            {
                                Id = dispositivo.Id,
                                Fabricante = dispositivo.Fabricante,
                                Modelo = dispositivo.Modelo
                            });
                        }
                        var conocimientosResult = await this.GetAll();

                        if (conocimientosResult.success && conocimientosResult.conocimientos.Count > 0)
                        {
                            var conocimientosTecnicoId = conocimientosResult.conocimientos.FindAll(conocimiento => conocimiento.TecnicoId == tecnicoId);

                            foreach (var conocimiento in conocimientosTecnicoId)
                            {
                                dispositivos.RemoveAll(dispositivo => dispositivo.Id == conocimiento.DispositivoId);
                            }
                        }

                        foreach (var dispositivo in dispositivos)
                        {
                            availableConocimientos.Add(new Conocimiento
                            {
                                TecnicoId = tecnicoId,
                                DispositivoId = dispositivo.Id ?? 0,
                                Tecnico = tecnico,
                                Dispositivo = dispositivo
                            });
                        }
                    }

                    return (true, "Lista disponible del técnico obtenida correctamente.", availableConocimientos);
                }

                return (true, $"No se ha encontrado el técnico con id {tecnicoId}.", availableConocimientos);
            }
            catch (Exception ex)
            {
                return (false, $"Error al obtener los dispositivos disponibles {ex.Message}", new List<Conocimiento>());
            }
        }

        public async Task<(bool success, string message, Conocimiento conocimiento)> GetById(int id)
        {
            try
            {
                var result = await _conocimientoRepository.GetById(id);

                return (result.Id > 0)
                    ? (true, "Conocimiento obtenido correctamente.", result)
                    : (true, $"No existe el conocimiento con ID: {id}.", result);

            }
            catch (Exception)
            {
                return (false, "Error al obtener los conocimientos.", new Conocimiento());
            }
        }

        public async Task<(bool success, string message, List<Conocimiento> conocimientos)> GetByDispositivoId(int dispositivoId)
        {
            if (dispositivoId <= 0)
            {
                return (false, "ID de dispositivo inválido.", new List<Conocimiento>());
            }

            try
            {
                var result = await _conocimientoRepository.GetByDispositivoId(dispositivoId);

                return (result.Count > 0)
                    ? (true, "Conocimientos obtenidos correctamente.", result)
                    : (true, $"No existen conocimientos para el dispositivo con ID: {dispositivoId}.", result);
            }
            catch (Exception)
            {
                return (false, "Error al obtener los conocimientos por dispositivo.", new List<Conocimiento>());
            }
        }
    }
}
