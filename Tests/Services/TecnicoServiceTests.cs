using Core.Interfaces;
using Core.Models;
using Core.Services;
using Moq;

namespace Tests.Services
{
    public class TecnicoServiceTests
    {
        private readonly Mock<ITecnicoRepository> _mockTecnicoRepository;
        private readonly TecnicoService _tecnicoService;

        public TecnicoServiceTests()
        {
            _mockTecnicoRepository = new Mock<ITecnicoRepository>();
            // inicializar el servicio con el repositorio simulado
            _tecnicoService = new TecnicoService(_mockTecnicoRepository.Object);
        }

        [Fact]
        public async void CreateSuccess()
        {
            // Arrange
            var tecnicoToCreate = new Tecnico
            {
                Nombre = "John",
                Apellidos = "Doe",
                Gaveta = 1,
                NombrePC = "John-PC",
                UsuarioPC = "john"
            };

            _mockTecnicoRepository.Setup(repository => repository.create(It.IsAny<Tecnico>())).ReturnsAsync(1);

            // Act
            var (success, message, id) = await _tecnicoService.create(tecnicoToCreate);

            // Assert
            Assert.True(success);
            Assert.Equal("Técnico creado correctamente con ID 1.", message);
            Assert.Equal(1, id);
        }

        [Fact]
        public async void UpdateSuccess()
        {
            // Arrange
            var tecnicoToUpdate = new Tecnico
            {
                Id = 1,
                Nombre = "John",
                Apellidos = "Doe",
                Gaveta = 1,
                NombrePC = "John-PC",
                UsuarioPC = "john"
            };

            _mockTecnicoRepository.Setup(repository => repository.update(It.IsAny<Tecnico>())).ReturnsAsync(true);

            // Act
            var (success, message) = await _tecnicoService.update(tecnicoToUpdate);

            // Assert
            Assert.True(success);
        }

        [Fact]
        public async void UpdateTecnicoNameEmptyFail()
        {
            // Arrange
            var tecnicoToUpdate = new Tecnico
            {
                Id = 1,
                Nombre = "",
                Apellidos = "Doe",
                Gaveta = 1,
                NombrePC = "John-PC",
                UsuarioPC = "john"
            };

            // Act
            var (success, message) = await _tecnicoService.update(tecnicoToUpdate);

            // Assert
            Assert.False(success);
            Assert.Equal("El nombre del técnico es obligatorio.", message);
        }

        [Fact]
        public async void UpdateTecnicoGavetaAlreadyAssignedFail()
        {
            // Arrange
            var tecnicoToUpdate = new Tecnico
            {
                Id = 1,
                Nombre = "John",
                Apellidos = "Doe",
                Gaveta = 2,
                NombrePC = "John-PC",
                UsuarioPC = "john"
            };

            var existingTecnico = new Tecnico
            {
                Id = 2,
                Nombre = "Jane",
                Apellidos = "Smith",
                Gaveta = 2,
                NombrePC = "Jane-PC",
                UsuarioPC = "jane"
            };

            _mockTecnicoRepository.Setup(repository => repository.getByGaveta(2)).ReturnsAsync(existingTecnico);

            // Act
            var (success, message) = await _tecnicoService.update(tecnicoToUpdate);

            // Assert
            Assert.False(success);
            Assert.Equal("La gaveta ya está asignada otro técnico.", message);
        }

        [Fact]
        public async void UpdateTecnicoNombrePCAlreadyAssignedFail()
        {
            //Arrange
            var tecnicoToUpdate = new Tecnico
            {
                Id = 1,
                Nombre = "John",
                Apellidos = "Doe",
                Gaveta = 2,
                NombrePC = "PC",
                UsuarioPC = "john"
            };

            var existingTecnico = new Tecnico
            {
                Id = 2,
                Nombre = "Jane",
                Apellidos = "Smith",
                Gaveta = 2,
                NombrePC = "PC",
                UsuarioPC = "jane"
            };

            _mockTecnicoRepository.Setup(repository => repository.getByNombrePC("PC")).ReturnsAsync(existingTecnico);

            // Act
            var (success, message) = await _tecnicoService.update(tecnicoToUpdate);

            Assert.False(success);
            Assert.Equal("El PC ya está asignado a otro técnico.", message);
        }

        [Fact]
        public async void UpdateTecnicoFail()
        {
            //Arrange
            var tecnicoToUpdate = new Tecnico
            {
                Id = 1,
                Nombre = "John",
                Apellidos = "Doe",
                Gaveta = 2,
                NombrePC = "PC",
                UsuarioPC = "john"
            };

            _mockTecnicoRepository.Setup(repository => repository.update(It.IsAny<Tecnico>())).ThrowsAsync(new Exception());

            //Act
            var (success, message) = await _tecnicoService.update(tecnicoToUpdate);

            //Assert
            Assert.False(success);
            Assert.Equal("Error al crear el técnico", message);
        }

        [Fact]
        public async void GetByIdSuccess()
        {
            //Arrange
            var tecnico = new Tecnico
            {
                Id = 1,
                Nombre = "John",
                Apellidos = "Doe",
                Gaveta = 1,
                NombrePC = "John-PC",
                UsuarioPC = "john"
            };
            _mockTecnicoRepository.Setup(repository => repository.getById(1)).ReturnsAsync(tecnico);

            //Act
            var (success, message, result) = await _tecnicoService.getById(1);

            //Assert
            Assert.True(success);
            Assert.Equal("Técnico obtenido correctamente.", message);
            Assert.Equal(tecnico, result);
        }

        [Fact]
        public async void DeleteFail()
        {
            //Arrange
            _mockTecnicoRepository.Setup(repository => repository.delete(1)).ReturnsAsync(false);

            //Act
            var (success, message) = await _tecnicoService.delete(1);

            //Assert
            Assert.False(success);
            Assert.Equal("No se encontró el técnico con el ID: 1.", message);
        }
    }
}