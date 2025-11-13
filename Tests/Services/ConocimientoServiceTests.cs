using Core.Interfaces;
using Core.Models;
using Core.Services;
using Moq;

namespace Tests.Services
{
    public class ConocimientoServiceTests
    {
        private readonly Mock<IConocimientoRepository> _mockConocimientoRepository;
        private readonly Mock<IDispositivoService> _mockDispositivoService;
        private readonly Mock<ITecnicoService> _mockTecnicoService;
        private readonly ConocimientoService _conocimientoService;

        public ConocimientoServiceTests()
        {
            _mockConocimientoRepository = new Mock<IConocimientoRepository>();
            _mockDispositivoService = new Mock<IDispositivoService>();
            _mockTecnicoService = new Mock<ITecnicoService>();
            _conocimientoService = new ConocimientoService(_mockConocimientoRepository.Object, _mockDispositivoService.Object, _mockTecnicoService.Object);
        }

        [Fact]
        public async Task GetAllSuccess()
        {
            //Arrange
            List<Conocimiento> conocimientos = new List<Conocimiento>();
            conocimientos.Add(new Conocimiento());
            conocimientos[0].Id = 1;
            conocimientos[0].TecnicoId = 1;
            conocimientos[0].DispositivoId = 1;
            conocimientos[0].Tecnico = new Tecnico { Id = 1, Nombre = "Juan", Apellidos = "García" };
            conocimientos[0].Dispositivo = new Dispositivo { Id = 1, Fabricante = "Fabricante 1", Modelo = "Modelo 1" };

            _mockConocimientoRepository.Setup(repository => repository.GetAll()).ReturnsAsync(conocimientos);

            //Act
            var (success, message, conocimientosList) = await _conocimientoService.GetAll();

            //Assert
            Assert.True(success);
            Assert.Contains("Conocimientos obtenidos correctamente.", message);
            Assert.Contains("Juan", conocimientosList[0].Tecnico.Nombre);
        }

        [Fact]
        public async Task CreateSuccess()
        {
            //Arrange
            var conocimiento = new Conocimiento
            {
                Id = 1,
                TecnicoId = 1,
                DispositivoId = 1,
                Tecnico = new Tecnico { Id = 1, Nombre = "Juan", Apellidos = "García" },
                Dispositivo = new Dispositivo { Id = 1, Fabricante = "Fabricante 1", Modelo = "Modelo 1" }
            };

            _mockConocimientoRepository.Setup(repository => repository.Create(conocimiento)).ReturnsAsync(1);

            //Act
            var (success, message, id) = await _conocimientoService.Create(conocimiento);

            //Assert
            Assert.True(success);
            Assert.Contains("Conocimiento asignado correctamente.", message);
            Assert.Equal(1, id);
        }

        [Fact]
        public async Task DeleteSuccess()
        {
            //Arrange
            _mockConocimientoRepository.Setup(repository => repository.Delete(1)).ReturnsAsync(true);

            //Act
            var (success, message) = await _conocimientoService.Delete(1);

            //Assert
            Assert.True(success);
            Assert.Contains("Conocimiento desasignado correctamente.", message);
        }

        [Fact]
        public async Task GetAvailableConocimientosByTecnicoIdSuccess()
        {
            //Arrange
            var tecnicoId = 1;
            var tecnico = new Tecnico { Id = tecnicoId, Nombre = "Juan", Apellidos = "García" };
            var dispositivos = new List<Dispositivo>
            {
                new Dispositivo { Id = 1, Fabricante = "Fabricante 1", Modelo = "Modelo 1" },
                new Dispositivo { Id = 2, Fabricante = "Fabricante 2", Modelo = "Modelo 2" }
            };
            var conocimientos = new List<Conocimiento>
            {
                new Conocimiento
                {
                    Id = 1,
                    TecnicoId = tecnicoId,
                    DispositivoId = 1,
                    Dispositivo = new Dispositivo { Id = 1, Fabricante = "Fabricante 1", Modelo = "Modelo 1" }
                }
            };
            _mockTecnicoService.Setup(service => service.getById(tecnicoId)).ReturnsAsync((true, "", tecnico));
            _mockDispositivoService.Setup(service => service.GetAll()).ReturnsAsync((true, "", dispositivos));
            _mockConocimientoRepository.Setup(repository => repository.GetAll()).ReturnsAsync(conocimientos);
            //Act
            var (success, message, conocimientosList) = await _conocimientoService.GetAvailableConocimientosByTecnicoId(tecnicoId);
            //Assert
            Assert.True(success);
            Assert.Contains("obtenida correctamente", message);
            Assert.Single(conocimientosList);
            Assert.Equal(2, conocimientosList[0].DispositivoId);
        }

        [Fact]
        public async Task GetAvailableConocimientosByTecnicoIdTecnicoNotFound()
        {
            //Arrange
            var tecnicoId = 999;
            _mockTecnicoService.Setup(service => service.getById(tecnicoId)).ReturnsAsync((false, "", null));
            //Act
            var (success, message, conocimientosList) = await _conocimientoService.GetAvailableConocimientosByTecnicoId(tecnicoId);
            //Assert
            Assert.True(success);
            Assert.Contains("No se ha encontrado", message);
            Assert.Empty(conocimientosList);
        }

        [Fact]
        public async Task GetAvailableConocimientosByTecnicoIdNoDevicesAvailable()
        {
            //Arrange
            var tecnicoId = 1;
            var tecnico = new Tecnico { Id = tecnicoId, Nombre = "Juan", Apellidos = "García" };
            var dispositivos = new List<Dispositivo>();
            _mockTecnicoService.Setup(service => service.getById(tecnicoId)).ReturnsAsync((true, "", tecnico));
            _mockDispositivoService.Setup(service => service.GetAll()).ReturnsAsync((true, "", dispositivos));
            //Act
            var (success, message, conocimientosList) = await _conocimientoService.GetAvailableConocimientosByTecnicoId(tecnicoId);
            //Assert
            Assert.True(success);
            Assert.Empty(conocimientosList);
        }

        [Fact]
        public async Task GetAvailableConocimientosByTecnicoIdAllDevicesAssigned()
        {
            //Arrange
            var tecnicoId = 1;
            var tecnico = new Tecnico { Id = tecnicoId, Nombre = "Juan", Apellidos = "García" };
            var dispositivos = new List<Dispositivo>
            {
                new Dispositivo { Id = 1, Fabricante = "Fabricante 1", Modelo = "Modelo 1" }
            };
            var conocimientos = new List<Conocimiento>
            {
                new Conocimiento
                {
                    Id = 1,
                    TecnicoId = tecnicoId,
                    Tecnico = tecnico,
                    DispositivoId = 1,
                    Dispositivo = new Dispositivo { Id = 1, Fabricante = "Fabricante 1", Modelo = "Modelo 1" }
                }
            };
            _mockTecnicoService.Setup(service => service.getById(tecnicoId)).ReturnsAsync((true, "", tecnico));
            _mockDispositivoService.Setup(service => service.GetAll()).ReturnsAsync((true, "", dispositivos));
            _mockConocimientoRepository.Setup(repository => repository.GetAll()).ReturnsAsync(conocimientos);
            //Act
            var (success, message, conocimientosList) = await _conocimientoService.GetAvailableConocimientosByTecnicoId(tecnicoId);
            //Assert
            Assert.True(success);
            Assert.Empty(conocimientosList);
        }

        [Fact]
        public async Task GetAvailableConocimientosByTecnicoIdExceptionThrown()
        {
            //Arrange
            var tecnicoId = 1;
            _mockTecnicoService.Setup(service => service.getById(tecnicoId)).ThrowsAsync(new Exception("Database error"));
            //Act
            var (success, message, conocimientosList) = await _conocimientoService.GetAvailableConocimientosByTecnicoId(tecnicoId);
            //Assert
            Assert.False(success);
            Assert.Contains("Error al obtener", message);
            Assert.Empty(conocimientosList);
        }
    }
}
