using Core.Interfaces;
using Core.Models;
using Core.Services;
using Moq;

namespace Tests.Services
{
    public class ActualizacionServiceTests
    {
        private readonly Mock<IActualizacionRepository> _mockActualizacionRepository;
        private readonly Mock<IDispositivoService> _mockDispositivoService;
        private readonly ActualizacionService _actualizacionService;

        public ActualizacionServiceTests()
        {
            _mockActualizacionRepository = new Mock<IActualizacionRepository>();
            _mockDispositivoService = new Mock<IDispositivoService>();
            _actualizacionService = new ActualizacionService(
                _mockActualizacionRepository.Object,
                _mockDispositivoService.Object);
        }

        [Fact]
        public async Task GetAllSuccess()
        {
            //Arrange
            List<Actualizacion> actualizaciones = new List<Actualizacion>();
            actualizaciones.Add(new Actualizacion());
            actualizaciones[0].Id = 1;
            actualizaciones[0].DispositivoId = 1;
            actualizaciones[0].Version = "1.0.0";
            actualizaciones[0].Descripcion = "Primera actualización";
            actualizaciones[0].Fecha = "2025-01-15 09:00:00";
            actualizaciones[0].Dispositivo = new Dispositivo { Id = 1, Fabricante = "Apple", Modelo = "iPhone 13" };

            _mockActualizacionRepository.Setup(repository => repository.GetAll()).ReturnsAsync(actualizaciones);

            //Act
            var (success, message, actualizacionesList) = await _actualizacionService.GetAll();

            //Assert
            Assert.True(success);
            Assert.Contains("Actualizaciones obtenidas correctamente.", message);
            Assert.Single(actualizacionesList);
            Assert.Equal("1.0.0", actualizacionesList[0].Version);
        }

        [Fact]
        public async Task GetAllEmptyList()
        {
            //Arrange
            _mockActualizacionRepository.Setup(repository => repository.GetAll()).ReturnsAsync(new List<Actualizacion>());

            //Act
            var (success, message, actualizacionesList) = await _actualizacionService.GetAll();

            //Assert
            Assert.False(success);
            Assert.Contains("No existen actualizaciones en la base de datos.", message);
            Assert.Null(actualizacionesList);
        }

        [Fact]
        public async Task CreateSuccess()
        {
            //Arrange
            var actualizacion = new Actualizacion
            {
                DispositivoId = 1,
                Version = "1.0.0",
                Descripcion = "Primera actualización",
                Fecha = "2025-01-15 09:00:00"
            };

            var dispositivo = new Dispositivo { Id = 1, Fabricante = "Apple", Modelo = "iPhone 13" };

            _mockDispositivoService.Setup(s => s.GetById(1))
                .ReturnsAsync((true, "Dispositivo obtenido correctamente.", dispositivo));
            _mockActualizacionRepository.Setup(r => r.GetByDispositivoId(1))
                .ReturnsAsync(new List<Actualizacion>());
            _mockActualizacionRepository.Setup(repository => repository.Create(actualizacion))
                .ReturnsAsync(1);

            //Act
            var (success, message, id) = await _actualizacionService.Create(actualizacion);

            //Assert
            Assert.True(success);
            Assert.Contains("Actualización creada correctamente con ID: 1.", message);
            Assert.Equal(1, id);
        }

        [Fact]
        public async Task CreateNullActualizacionFail()
        {
            //Act
            var (success, message, id) = await _actualizacionService.Create(null);

            //Assert
            Assert.False(success);
            Assert.Contains("Los datos de la actualización no pueden estar vacíos.", message);
            Assert.Null(id);
        }

        [Fact]
        public async Task CreateInvalidDispositivoIdFail()
        {
            //Arrange
            var actualizacion = new Actualizacion
            {
                DispositivoId = 0,
                Version = "1.0.0",
                Descripcion = "Test",
                Fecha = "2025-01-15 09:00:00"
            };

            //Act
            var (success, message, id) = await _actualizacionService.Create(actualizacion);

            //Assert
            Assert.False(success);
            Assert.Contains("El ID del dispositivo es obligatorio.", message);
            Assert.Null(id);
        }

        [Fact]
        public async Task CreateEmptyVersionFail()
        {
            //Arrange
            var actualizacion = new Actualizacion
            {
                DispositivoId = 1,
                Version = "",
                Descripcion = "Test",
                Fecha = "2025-01-15 09:00:00"
            };

            //Act
            var (success, message, id) = await _actualizacionService.Create(actualizacion);

            //Assert
            Assert.False(success);
            Assert.Contains("El campo versión es obligatorio.", message);
            Assert.Null(id);
        }

        [Fact]
        public async Task CreateEmptyDescripcionFail()
        {
            //Arrange
            var actualizacion = new Actualizacion
            {
                DispositivoId = 1,
                Version = "1.0.0",
                Descripcion = "",
                Fecha = "2025-01-15 09:00:00"
            };

            //Act
            var (success, message, id) = await _actualizacionService.Create(actualizacion);

            //Assert
            Assert.False(success);
            Assert.Contains("El campo descripción es obligatorio.", message);
            Assert.Null(id);
        }

        [Fact]
        public async Task CreateEmptyFechaFail()
        {
            //Arrange
            var actualizacion = new Actualizacion
            {
                DispositivoId = 1,
                Version = "1.0.0",
                Descripcion = "Test",
                Fecha = ""
            };

            //Act
            var (success, message, id) = await _actualizacionService.Create(actualizacion);

            //Assert
            Assert.False(success);
            Assert.Contains("El campo fecha es obligatorio.", message);
            Assert.Null(id);
        }

        [Fact]
        public async Task CreateInvalidDateFormatFail()
        {
            //Arrange
            var actualizacion = new Actualizacion
            {
                DispositivoId = 1,
                Version = "1.0.0",
                Descripcion = "Test",
                Fecha = "formato inválido"
            };

            //Act
            var (success, message, id) = await _actualizacionService.Create(actualizacion);

            //Assert
            Assert.False(success);
            Assert.Contains("Formato de fecha inválido.", message);
            Assert.Null(id);
        }

        [Fact]
        public async Task CreateDateNormalizationSuccess()
        {
            //Arrange
            var actualizacion = new Actualizacion
            {
                DispositivoId = 1,
                Version = "1.0.0",
                Descripcion = "Test",
                Fecha = "2025-01-15"
            };

            var dispositivo = new Dispositivo { Id = 1, Fabricante = "Apple", Modelo = "iPhone 13" };

            _mockDispositivoService.Setup(s => s.GetById(1))
                .ReturnsAsync((true, "Dispositivo obtenido correctamente.", dispositivo));
            _mockActualizacionRepository.Setup(r => r.GetByDispositivoId(1))
                .ReturnsAsync(new List<Actualizacion>());
            _mockActualizacionRepository.Setup(repository => repository.Create(It.IsAny<Actualizacion>()))
                .ReturnsAsync(1);

            //Act
            var (success, message, id) = await _actualizacionService.Create(actualizacion);

            //Assert
            Assert.True(success);

            _mockActualizacionRepository.Verify(r => r.Create(It.Is<Actualizacion>(a => a.Fecha.Contains(" "))), Times.Once);
        }

        [Fact]
        public async Task CreateDispositivoNotFoundFail()
        {
            //Arrange
            var actualizacion = new Actualizacion
            {
                DispositivoId = 999,
                Version = "1.0.0",
                Descripcion = "Test",
                Fecha = "2025-01-15 09:00:00"
            };

            _mockDispositivoService.Setup(s => s.GetById(999))
                .ReturnsAsync((false, "No se encontró un dispositivo con id 999.", null));

            //Act
            var (success, message, id) = await _actualizacionService.Create(actualizacion);

            //Assert
            Assert.False(success);
            Assert.Contains("No existe un dispositivo con ID: 999.", message);
            Assert.Null(id);
        }

        [Fact]
        public async Task CreateDuplicateVersionForDispositivoFail()
        {
            //Arrange
            var actualizacion = new Actualizacion
            {
                DispositivoId = 1,
                Version = "1.0.0",
                Descripcion = "Test",
                Fecha = "2025-01-15 09:00:00"
            };

            var dispositivo = new Dispositivo { Id = 1, Fabricante = "Apple", Modelo = "iPhone 13" };

            var existingActualizaciones = new List<Actualizacion>
            {
                new Actualizacion { Id = 1, DispositivoId = 1, Version = "1.0.0" }
            };

            _mockDispositivoService.Setup(s => s.GetById(1))
                .ReturnsAsync((true, "Dispositivo obtenido correctamente.", dispositivo));
            _mockActualizacionRepository.Setup(r => r.GetByDispositivoId(1))
                .ReturnsAsync(existingActualizaciones);

            //Act
            var (success, message, id) = await _actualizacionService.Create(actualizacion);

            //Assert
            Assert.False(success);
            Assert.Contains("Ya existe una actualización con la versión 1.0.0 para este dispositivo.", message);
            Assert.Null(id);
        }

        [Fact]
        public async Task DeleteSuccess()
        {
            //Arrange
            _mockActualizacionRepository.Setup(repository => repository.Delete(1)).ReturnsAsync(true);

            //Act
            var (success, message) = await _actualizacionService.Delete(1);

            //Assert
            Assert.True(success);
            Assert.Contains("Actualización eliminada correctamente.", message);
        }

        [Fact]
        public async Task DeleteInvalidIdFail()
        {
            //Act
            var (success, message) = await _actualizacionService.Delete(0);

            //Assert
            Assert.False(success);
            Assert.Contains("ID de actualización inválido.", message);
        }

        [Fact]
        public async Task DeleteNotFoundFail()
        {
            //Arrange
            _mockActualizacionRepository.Setup(repository => repository.Delete(999)).ReturnsAsync(false);

            //Act
            var (success, message) = await _actualizacionService.Delete(999);

            //Assert
            Assert.False(success);
            Assert.Contains("No se encontró la actualización con ID: 999.", message);
        }

        [Fact]
        public async Task GetByIdSuccess()
        {
            //Arrange
            var actualizacion = new Actualizacion
            {
                Id = 1,
                DispositivoId = 1,
                Version = "1.0.0",
                Descripcion = "Primera actualización",
                Fecha = "2025-01-15 09:00:00",
                Dispositivo = new Dispositivo { Id = 1, Fabricante = "Apple", Modelo = "iPhone 13" }
            };

            _mockActualizacionRepository.Setup(repository => repository.GetById(1)).ReturnsAsync(actualizacion);

            //Act
            var (success, message, result) = await _actualizacionService.GetById(1);

            //Assert
            Assert.True(success);
            Assert.Contains("Actualización obtenida correctamente.", message);
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("1.0.0", result.Version);
        }

        [Fact]
        public async Task GetByIdInvalidIdFail()
        {
            //Act
            var (success, message, result) = await _actualizacionService.GetById(0);

            //Assert
            Assert.False(success);
            Assert.Contains("ID de actualización inválido.", message);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdNotFoundFail()
        {
            //Arrange
            var actualizacion = new Actualizacion { Id = 0 };
            _mockActualizacionRepository.Setup(repository => repository.GetById(999)).ReturnsAsync(actualizacion);

            //Act
            var (success, message, result) = await _actualizacionService.GetById(999);

            //Assert
            Assert.False(success);
            Assert.Contains("No se encontró la actualización con ID: 999.", message);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByDispositivoIdSuccess()
        {
            //Arrange
            var actualizaciones = new List<Actualizacion>
            {
                new Actualizacion { Id = 1, DispositivoId = 1, Version = "1.0.0" },
                new Actualizacion { Id = 2, DispositivoId = 1, Version = "2.0.0" }
            };

            _mockActualizacionRepository.Setup(repository => repository.GetByDispositivoId(1))
                .ReturnsAsync(actualizaciones);

            //Act
            var (success, message, result) = await _actualizacionService.GetByDispositivoId(1);

            //Assert
            Assert.True(success);
            Assert.Contains("Actualizaciones obtenidas correctamente.", message);
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByDispositivoIdInvalidIdFail()
        {
            //Act
            var (success, message, result) = await _actualizacionService.GetByDispositivoId(0);

            //Assert
            Assert.False(success);
            Assert.Contains("ID de dispositivo inválido.", message);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByDispositivoIdEmptyListFail()
        {
            //Arrange
            _mockActualizacionRepository.Setup(repository => repository.GetByDispositivoId(999))
                .ReturnsAsync(new List<Actualizacion>());

            //Act
            var (success, message, result) = await _actualizacionService.GetByDispositivoId(999);

            //Assert
            Assert.False(success);
            Assert.Contains("No existen actualizaciones para el dispositivo con ID: 999.", message);
            Assert.Null(result);
        }
    }
}
