using Core.Interfaces;
using Core.Models;
using Core.Services;
using Moq;

namespace Tests.Services
{
    public class VerificacionServiceTests
    {
        private readonly Mock<IVerificacionRepository> _mockVerificacionRepository;
        private readonly VerificacionService _verificacionService;

        public VerificacionServiceTests()
        {
            _mockVerificacionRepository = new Mock<IVerificacionRepository>();
            _verificacionService = new VerificacionService(_mockVerificacionRepository.Object);
        }

        [Fact]
        public async Task GetAllSuccess()
        {
            //Arrange
            List<Verificacion> verificaciones = new List<Verificacion>();
            verificaciones.Add(new Verificacion());
            verificaciones[0].Id = 1;
            verificaciones[0].ActualizacionId = 1;
            verificaciones[0].TecnicoId = 1;
            verificaciones[0].Confirmado = 0;
            verificaciones[0].FechaConfirmacion = string.Empty;
            verificaciones[0].Tecnico = new Tecnico { Id = 1, Nombre = "Juan", Apellidos = "García" };

            _mockVerificacionRepository.Setup(repository => repository.GetAll()).ReturnsAsync(verificaciones);

            //Act
            var (success, message, verificacionesList) = await _verificacionService.GetAll();

            //Assert
            Assert.True(success);
            Assert.Contains("Verificaciones obtenidas correctamente.", message);
            Assert.Single(verificacionesList);
            Assert.Equal(1, verificacionesList[0].ActualizacionId);
        }

        [Fact]
        public async Task GetAllEmptySuccess()
        {
            //Arrange
            _mockVerificacionRepository.Setup(repository => repository.GetAll()).ReturnsAsync(new List<Verificacion>());

            //Act
            var (success, message, verificacionesList) = await _verificacionService.GetAll();

            //Assert
            Assert.True(success);
            Assert.Contains("No existen registros de verificaciones.", message);
            Assert.Empty(verificacionesList);
        }

        [Fact]
        public async Task CreateSuccess()
        {
            //Arrange
            var verificacion = new Verificacion
            {
                ActualizacionId = 1,
                TecnicoId = 1,
                Confirmado = 0,
                FechaConfirmacion = string.Empty
            };

            _mockVerificacionRepository.Setup(repository => repository.Create(verificacion)).ReturnsAsync(1);

            //Act
            var (success, message, id) = await _verificacionService.Create(verificacion);

            //Assert
            Assert.True(success);
            Assert.Contains("Verificación asignada correctamente.", message);
            Assert.Equal(1, id);
        }

        [Fact]
        public async Task CreateNullFails()
        {
            //Act
            var (success, message, id) = await _verificacionService.Create(null);

            //Assert
            Assert.False(success);
            Assert.Contains("Los datos de la verificación no pueden estar vacíos.", message);
            Assert.Equal(0, id);
        }

        [Fact]
        public async Task CreateInvalidActualizacionIdFails()
        {
            //Arrange
            var verificacion = new Verificacion
            {
                ActualizacionId = 0,
                TecnicoId = 1,
                Confirmado = 0,
                FechaConfirmacion = string.Empty
            };

            //Act
            var (success, message, id) = await _verificacionService.Create(verificacion);

            //Assert
            Assert.False(success);
            Assert.Contains("El ID de la actualización es obligatorio.", message);
            Assert.Equal(0, id);
        }

        [Fact]
        public async Task CreateInvalidTecnicoIdFails()
        {
            //Arrange
            var verificacion = new Verificacion
            {
                ActualizacionId = 1,
                TecnicoId = 0,
                Confirmado = 0,
                FechaConfirmacion = string.Empty
            };

            //Act
            var (success, message, id) = await _verificacionService.Create(verificacion);

            //Assert
            Assert.False(success);
            Assert.Contains("El ID del técnico es obligatorio.", message);
            Assert.Equal(0, id);
        }

        [Fact]
        public async Task ConfirmVerificationSuccess()
        {
            //Arrange
            _mockVerificacionRepository.Setup(repository => repository.ConfirmVerification(1, 1, It.IsAny<string>()))
                .ReturnsAsync(true);

            //Act
            var (success, message) = await _verificacionService.ConfirmVerification(1, 1);

            //Assert
            Assert.True(success);
            Assert.Contains("Verificación confirmada correctamente.", message);
        }

        [Fact]
        public async Task ConfirmVerificationInvalidActualizacionIdFails()
        {
            //Act
            var (success, message) = await _verificacionService.ConfirmVerification(0, 1);

            //Assert
            Assert.False(success);
            Assert.Contains("ID de actualización inválido.", message);
        }

        [Fact]
        public async Task ConfirmVerificationInvalidTecnicoIdFails()
        {
            //Act
            var (success, message) = await _verificacionService.ConfirmVerification(1, 0);

            //Assert
            Assert.False(success);
            Assert.Contains("ID de técnico inválido.", message);
        }

        [Fact]
        public async Task ConfirmVerificationNotFoundFails()
        {
            //Arrange
            _mockVerificacionRepository.Setup(repository => repository.ConfirmVerification(999, 999, It.IsAny<string>()))
                .ReturnsAsync(false);

            //Act
            var (success, message) = await _verificacionService.ConfirmVerification(999, 999);

            //Assert
            Assert.False(success);
            Assert.Contains("No se encontró la verificación para actualización 999 y técnico 999.", message);
        }

        [Fact]
        public async Task GetByIdSuccess()
        {
            //Arrange
            var verificacion = new Verificacion
            {
                Id = 1,
                ActualizacionId = 1,
                TecnicoId = 1,
                Confirmado = 1,
                FechaConfirmacion = "2025-01-15 09:00:00",
                Tecnico = new Tecnico { Id = 1, Nombre = "Juan", Apellidos = "García" }
            };

            _mockVerificacionRepository.Setup(repository => repository.GetById(1)).ReturnsAsync(verificacion);

            //Act
            var (success, message, result) = await _verificacionService.GetById(1);

            //Assert
            Assert.True(success);
            Assert.Contains("Verificación obtenida correctamente.", message);
            Assert.Equal(1, result.Id);
            Assert.Equal(1, result.Confirmado);
        }

        [Fact]
        public async Task GetByIdNotFoundSuccess()
        {
            //Arrange
            var verificacion = new Verificacion { Id = 0 };
            _mockVerificacionRepository.Setup(repository => repository.GetById(999)).ReturnsAsync(verificacion);

            //Act
            var (success, message, result) = await _verificacionService.GetById(999);

            //Assert
            Assert.True(success);
            Assert.Contains("No existe la verificación con ID: 999.", message);
            Assert.Equal(0, result.Id);
        }

        [Fact]
        public async Task GetByActualizacionIdSuccess()
        {
            //Arrange
            var verificaciones = new List<Verificacion>
            {
                new Verificacion { Id = 1, ActualizacionId = 1, TecnicoId = 1 },
                new Verificacion { Id = 2, ActualizacionId = 1, TecnicoId = 2 }
            };

            _mockVerificacionRepository.Setup(repository => repository.GetByActualizacionId(1))
                .ReturnsAsync(verificaciones);

            //Act
            var (success, message, result) = await _verificacionService.GetByActualizacionId(1);

            //Assert
            Assert.True(success);
            Assert.Contains("Verificaciones obtenidas correctamente.", message);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByActualizacionIdInvalidIdFails()
        {
            //Act
            var (success, message, result) = await _verificacionService.GetByActualizacionId(0);

            //Assert
            Assert.False(success);
            Assert.Contains("ID de actualización inválido.", message);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByActualizacionIdEmptySuccess()
        {
            //Arrange
            _mockVerificacionRepository.Setup(repository => repository.GetByActualizacionId(999))
                .ReturnsAsync(new List<Verificacion>());

            //Act
            var (success, message, result) = await _verificacionService.GetByActualizacionId(999);

            //Assert
            Assert.True(success);
            Assert.Contains("No existen verificaciones para la actualización con ID: 999.", message);
            Assert.Empty(result);
        }
    }
}
