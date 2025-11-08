using Core.Interfaces;
using Core.Models;
using Core.Services;
using Moq;

namespace Tests.Services
{
    public class DispositivoServiceTests
    {
        private readonly Mock<IDispositivoRepository> _mockDispositivoRepository;
        private readonly IDispositivoService _dispositivoService;
        public DispositivoServiceTests()
        {
            _mockDispositivoRepository = new Mock<IDispositivoRepository>();
            _dispositivoService = new DispositivoService(_mockDispositivoRepository.Object);
        }

        [Fact]
        public async Task CreateSuccess()
        {
            //Arrange
            var newDispositivo = new Dispositivo
            {
                Fabricante = "Fabricante 1",
                Modelo = "Modelo 1"
            };

            _mockDispositivoRepository.Setup(repository => repository.Create(It.IsAny<Dispositivo>())).ReturnsAsync(1);

            //Act
            var result = await _dispositivoService.Create(newDispositivo);

            //Assert
            Assert.True(result.success);
            Assert.Equal("Dispositivo creado correctamente con ID: 1", result.message);
            Assert.Equal(1, result.id);
        }

        [Fact]
        public async Task CreateFabricanteModeloAlreadyExistsFail()
        {
            //Arrange
            var dispositivo = new Dispositivo
            {
                Id = 1,
                Fabricante = "Fabricante 1",
                Modelo = "Modelo 1"
            };

            var newDispositivo = new Dispositivo
            {
                Fabricante = "Fabricante 1",
                Modelo = "Modelo 1"
            };

            _mockDispositivoRepository.Setup(repository => repository.GetByFabricanteModelo("Fabricante 1", "Modelo 1")).ReturnsAsync(dispositivo);

            //Act
            var result = await _dispositivoService.Create(newDispositivo);

            //Assert
            Assert.False(result.success);
            Assert.Equal("El dispositivo ya está registrado en la base de datos.", result.message);
            Assert.Null(result.id);
        }

        [Fact]
        public async Task DeleteDispositivoSuccess()
        {
            //Arrange

            _mockDispositivoRepository.Setup(repository => repository.Delete(1)).ReturnsAsync(true);

            //Act
            var result = await _dispositivoService.Delete(1);

            //Assert
            Assert.True(result.success);
            Assert.Equal("Dispositivo eliminado correctamente.", result.message);
        }

        [Fact]
        public async Task DeleteDispositivoInvalidIdFail()
        {
            //Arrange

            _mockDispositivoRepository.Setup(repository => repository.Delete(2)).ReturnsAsync(false);

            //Act
            var result = await _dispositivoService.Delete(2);

            //Assert
            Assert.False(result.success);
            Assert.Equal("No se encontró el técnico con ID: 2", result.message);
        }

        [Fact]
        public async Task GetByIdSuccess()
        {
            //Arrange
            var dispositivo = new Dispositivo
            {
                Id = 2,
                Fabricante = "Fabricante 2",
                Modelo = "Modelo 2"
            };

            _mockDispositivoRepository.Setup(repository => repository.GetById(2)).ReturnsAsync(dispositivo);

            //Act
            var (success, message, dispositivoById) = await _dispositivoService.GetById(2);

            //Assert
            Assert.True(success);
            Assert.Equal("Fabricante 2", dispositivoById.Fabricante);
            Assert.Equal("Modelo 2", dispositivoById.Modelo);
        }

        [Fact]
        public async Task UpdateSuccess()
        {
            //Arrange
            var dispositivoToUpdate = new Dispositivo
            {
                Id = 2,
                Fabricante = "Fabricante 3",
                Modelo = "Modelo 3"
            };

            _mockDispositivoRepository.Setup(repository => repository.GetByFabricanteModelo("Fabricante 3", "Modelo 3")).ReturnsAsync((Dispositivo)null);

            _mockDispositivoRepository.Setup(repository => repository.Update(dispositivoToUpdate)).ReturnsAsync(true);

            //Act
            var (success, message) = await _dispositivoService.Update(dispositivoToUpdate);

            //Assert
            Assert.True(success);
            Assert.Equal("Dispositivo actualizado correctamente.", message);
        }

        [Fact]
        public async Task UpdateFail()
        {
            //Arrange
            var dispositivo = new Dispositivo
            {
                Id = 3,
                Fabricante = "Fabricante 3",
                Modelo = "Modelo 3"
            };
            var dispositivoToUpdate = new Dispositivo
            {
                Id = 1,
                Fabricante = "Fabricante 3",
                Modelo = "Modelo 3"
            };

            _mockDispositivoRepository.Setup(repository => repository.GetByFabricanteModelo("Fabricante 3", "Modelo 3")).ReturnsAsync(dispositivo);

            //Act
            var (success, message) = await _dispositivoService.Update(dispositivoToUpdate);

            //Assert
            Assert.False(success);
            Assert.Equal("El dispositivo ya existe en la base de datos.", message);
        }
    }
}
