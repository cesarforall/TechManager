using Core.Models;
using Data.Repositories;
using Microsoft.Data.Sqlite;

namespace Tests.Repositories
{
    public class VerificacionRepositoryTests : IDisposable
    {
        private readonly VerificacionRepository _verificacionRepository;
        private readonly string _connectionString;
        private readonly SqliteConnection _connection;

        public VerificacionRepositoryTests()
        {
            _connectionString = "Data Source=TestDB;Mode=Memory;Cache=Shared";
            _connection = new SqliteConnection(_connectionString);
            _connection.Open();
            InitializeDatabase();
            _verificacionRepository = new VerificacionRepository(_connection.ConnectionString);
        }

        private void InitializeDatabase()
        {
            using var command = _connection.CreateCommand();
            command.CommandText = """
                CREATE TABLE IF NOT EXISTS tecnicos (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    nombre TEXT NOT NULL,
                    apellidos TEXT NOT NULL,
                    gaveta INTEGER UNIQUE,
                    nombre_pc TEXT UNIQUE,
                    usuario_pc TEXT
                );
                CREATE TABLE IF NOT EXISTS dispositivos (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    fabricante TEXT NOT NULL,
                    modelo TEXT NOT NULL,
                    UNIQUE (fabricante, modelo)
                );
                CREATE TABLE IF NOT EXISTS actualizaciones (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    dispositivo_id INTEGER NOT NULL,
                    version TEXT NOT NULL,
                    descripcion TEXT NOT NULL,
                    fecha TEXT NOT NULL,
                    UNIQUE(dispositivo_id, version),
                    FOREIGN KEY (dispositivo_id) REFERENCES dispositivos(id) ON DELETE CASCADE
                );
                CREATE TABLE IF NOT EXISTS verificaciones (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    actualizacion_id INTEGER NOT NULL,
                    tecnico_id INTEGER NOT NULL,
                    confirmado INTEGER NOT NULL DEFAULT 0,
                    fecha_conf TEXT,
                    UNIQUE(actualizacion_id, tecnico_id),
                    FOREIGN KEY (actualizacion_id) REFERENCES actualizaciones(id) ON DELETE CASCADE,
                    FOREIGN KEY (tecnico_id) REFERENCES tecnicos(id) ON DELETE CASCADE
                );
                """;
            command.ExecuteNonQuery();
        }

        [Fact]
        public async Task GetAllSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc) VALUES ('James', 'Smith', 1, 'John-PC', 'john');
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc) VALUES ('Alice', 'Johnson', 2, 'Alice-PC', 'alice');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 1', 'Modelo 1');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 2', 'Modelo 2');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (1, '1.0.0', 'Primera actualización', '2025-01-15 09:00:00');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (2, '2.0.0', 'Segunda actualización', '2025-01-16 14:30:00');
                INSERT INTO verificaciones (actualizacion_id, tecnico_id, confirmado, fecha_conf) VALUES (1, 1, 0, NULL);
                INSERT INTO verificaciones (actualizacion_id, tecnico_id, confirmado, fecha_conf) VALUES (1, 2, 1, '2025-01-17 11:15:00');
                INSERT INTO verificaciones (actualizacion_id, tecnico_id, confirmado, fecha_conf) VALUES (2, 1, 0, NULL);
                """;
            command.ExecuteNonQuery();

            //Act
            var result = await _verificacionRepository.GetAll();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal(1, result[0].ActualizacionId);
            Assert.Equal(1, result[0].TecnicoId);
            Assert.Equal("James", result[0].Tecnico.Nombre);
            Assert.Equal("Smith", result[0].Tecnico.Apellidos);
            Assert.Equal(0, result[0].Confirmado);
            Assert.Equal(string.Empty, result[0].FechaConfirmacion);
            Assert.Equal(2, result[1].TecnicoId);
            Assert.Equal("Alice", result[1].Tecnico.Nombre);
            Assert.Equal(1, result[1].Confirmado);
            Assert.Equal("2025-01-17 11:15:00", result[1].FechaConfirmacion);
        }

        [Fact]
        public async Task CreateSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc) VALUES ('James', 'Smith', 1, 'John-PC', 'john');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 1', 'Modelo 1');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (1, '1.0.0', 'Primera actualización', '2025-01-15 09:00:00');
                """;
            command.ExecuteNonQuery();

            Verificacion newVerificacion = new Verificacion
            {
                ActualizacionId = 1,
                TecnicoId = 1,
                Confirmado = 0,
                FechaConfirmacion = string.Empty
            };

            //Act
            var result = await _verificacionRepository.Create(newVerificacion);

            //Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task DeleteActualizacionCascadeSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc) VALUES ('James', 'Smith', 1, 'John-PC', 'john');
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc) VALUES ('Alice', 'Johnson', 2, 'Alice-PC', 'alice');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 1', 'Modelo 1');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 2', 'Modelo 2');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (1, '1.0.0', 'Primera actualización', '2025-01-15 09:00:00');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (2, '2.0.0', 'Segunda actualización', '2025-01-16 14:30:00');
                INSERT INTO verificaciones (actualizacion_id, tecnico_id, confirmado, fecha_conf) VALUES (1, 1, 0, NULL);
                INSERT INTO verificaciones (actualizacion_id, tecnico_id, confirmado, fecha_conf) VALUES (1, 2, 1, '2025-01-17 11:15:00');
                INSERT INTO verificaciones (actualizacion_id, tecnico_id, confirmado, fecha_conf) VALUES (2, 1, 0, NULL);
                DELETE FROM actualizaciones WHERE id = 1;
                """;
            command.ExecuteNonQuery();

            //Act
            var result = await _verificacionRepository.GetAll();

            //Assert
            Assert.Single(result);
            Assert.Equal(2, result[0].ActualizacionId);
            Assert.Equal(1, result[0].TecnicoId);
            Assert.Equal("James", result[0].Tecnico.Nombre);
        }

        [Fact]
        public async Task DeleteTecnicoCascadeSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc) VALUES ('James', 'Smith', 1, 'John-PC', 'john');
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc) VALUES ('Alice', 'Johnson', 2, 'Alice-PC', 'alice');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 1', 'Modelo 1');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (1, '1.0.0', 'Primera actualización', '2025-01-15 09:00:00');
                INSERT INTO verificaciones (actualizacion_id, tecnico_id, confirmado, fecha_conf) VALUES (1, 1, 0, NULL);
                INSERT INTO verificaciones (actualizacion_id, tecnico_id, confirmado, fecha_conf) VALUES (1, 2, 1, '2025-01-17 11:15:00');
                DELETE FROM tecnicos WHERE id = 2;
                """;
            command.ExecuteNonQuery();

            //Act
            var result = await _verificacionRepository.GetAll();

            //Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].ActualizacionId);
            Assert.Equal(1, result[0].TecnicoId);
            Assert.Equal("James", result[0].Tecnico.Nombre);
        }

        [Fact]
        public async Task GetByIdSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc) VALUES ('James', 'Smith', 1, 'John-PC', 'john');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 1', 'Modelo 1');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (1, '1.0.0', 'Primera actualización', '2025-01-15 09:00:00');
                INSERT INTO verificaciones (actualizacion_id, tecnico_id, confirmado, fecha_conf) VALUES (1, 1, 1, '2025-01-18 12:15:00');
                """;
            command.ExecuteNonQuery();

            //Act
            var result = await _verificacionRepository.GetById(1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(1, result.ActualizacionId);
            Assert.Equal(1, result.TecnicoId);
            Assert.Equal(1, result.Confirmado);
            Assert.Equal("2025-01-18 12:15:00", result.FechaConfirmacion);
            Assert.Equal("James", result.Tecnico.Nombre);
            Assert.Equal("Smith", result.Tecnico.Apellidos);
        }

        [Fact]
        public async Task GetByActualizacionIdSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc) VALUES ('James', 'Smith', 1, 'John-PC', 'john');
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc) VALUES ('Alice', 'Johnson', 2, 'Alice-PC', 'alice');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 1', 'Modelo 1');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 2', 'Modelo 2');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (1, '1.0.0', 'Primera actualización', '2025-01-15 09:00:00');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (2, '2.0.0', 'Segunda actualización', '2025-01-16 14:30:00');
                INSERT INTO verificaciones (actualizacion_id, tecnico_id, confirmado, fecha_conf) VALUES (1, 1, 0, NULL);
                INSERT INTO verificaciones (actualizacion_id, tecnico_id, confirmado, fecha_conf) VALUES (1, 2, 1, '2025-01-17 11:15:00');
                INSERT INTO verificaciones (actualizacion_id, tecnico_id, confirmado, fecha_conf) VALUES (2, 1, 0, NULL);
                """;
            command.ExecuteNonQuery();

            //Act
            var result = await _verificacionRepository.GetByActualizacionId(1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].ActualizacionId);
            Assert.Equal(1, result[0].TecnicoId);
            Assert.Equal("James", result[0].Tecnico.Nombre);
            Assert.Equal(0, result[0].Confirmado);
            Assert.Equal(1, result[1].ActualizacionId);
            Assert.Equal(2, result[1].TecnicoId);
            Assert.Equal("Alice", result[1].Tecnico.Nombre);
            Assert.Equal(1, result[1].Confirmado);
            Assert.Equal("2025-01-17 11:15:00", result[1].FechaConfirmacion);
        }

        [Fact]
        public async Task ConfirmVerificationSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc) VALUES ('James', 'Smith', 1, 'John-PC', 'john');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 1', 'Modelo 1');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (1, '1.0.0', 'Primera actualización', '2025-01-15 09:00:00');
                INSERT INTO verificaciones (actualizacion_id, tecnico_id, confirmado, fecha_conf) VALUES (1, 1, 0, NULL);
                """;
            command.ExecuteNonQuery();

            //Act
            var result = await _verificacionRepository.ConfirmVerification(1, 1, "2025-01-18 12:15:00");

            //Assert
            Assert.True(result);

            var verificacion = await _verificacionRepository.GetById(1);
            Assert.Equal(1, verificacion.Confirmado);
            Assert.Equal("2025-01-18 12:15:00", verificacion.FechaConfirmacion);
        }

        [Fact]
        public async Task ConfirmVerificationNotFoundFail()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc) VALUES ('James', 'Smith', 1, 'John-PC', 'john');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 1', 'Modelo 1');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (1, '1.0.0', 'Primera actualización', '2025-01-15 09:00:00');
                """;
            command.ExecuteNonQuery();

            //Act
            var result = await _verificacionRepository.ConfirmVerification(1, 1, "2025-01-18 12:15:00");

            //Assert
            Assert.False(result);
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
