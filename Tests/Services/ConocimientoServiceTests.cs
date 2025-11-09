using Core.Interfaces;
using Core.Models;
using Core.Services;
using Moq;

namespace Tests.Services
{
    public class ConocimientoServiceTests
    {
        private readonly Mock<IConocimientoRepository> _mockConocimientoRepository;
        private readonly IConocimientoService _conocimientoService;

        public ConocimientoServiceTests()
        {
            _mockConocimientoRepository = new Mock<IConocimientoRepository>();
            _conocimientoService = new ConocimientoService(_mockConocimientoRepository.Object);
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
    }
}
