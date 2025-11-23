using Core.Models;
using Data.Repositories;
using Microsoft.Data.Sqlite;

namespace Tests.Repositories
{
    public class ActualizacionRepositoryTests : IDisposable
    {
        private readonly ActualizacionRepository _actualizacionRepository;
        private readonly string _connectionString;
        private readonly SqliteConnection _connection;

        public ActualizacionRepositoryTests()
        {
            _connectionString = "Data Source=TestDB;Mode=Memory;Cache=Shared";
            _connection = new SqliteConnection(_connectionString);
            _connection.Open();
            InitializeDatabase();
            _actualizacionRepository = new ActualizacionRepository(_connection.ConnectionString);
        }

        private void InitializeDatabase()
        {
            using var command = _connection.CreateCommand();
            command.CommandText = """
                CREATE TABLE IF NOT EXISTS dispositivos (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    fabricante TEXT NOT NULL,
                    modelo TEXT NOT NULL,
                    UNIQUE (fabricante, modelo)
                );
                CREATE TABLE IF NOT EXISTS tecnicos (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    nombre TEXT NOT NULL,
                    apellidos TEXT NOT NULL,
                    gaveta INTEGER UNIQUE,
                    nombre_pc TEXT UNIQUE,
                    usuario_pc TEXT
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
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Apple', 'iPhone 13');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Samsung', 'Galaxy S21');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Google', 'Pixel 6');
                INSERT INTO tecnicos (nombre, apellidos) VALUES ('Juan', 'García');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (1, '1.0.0', 'Primera actualización', '2025-01-15 09:00:00');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (2, '2.0.0', 'Segunda actualización', '2025-01-16 14:30:00');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (3, '3.0.0', 'Tercera actualización', '2025-01-17 11:15:00');
                INSERT INTO verificaciones (actualizacion_id, tecnico_id, confirmado, fecha_conf) VALUES (1, 1, 0, NULL);
                INSERT INTO verificaciones (actualizacion_id, tecnico_id, confirmado, fecha_conf) VALUES (2, 1, 1, '2025-01-18 12:15:00');
                """;
            command.ExecuteNonQuery();

            //Act
            var result = await _actualizacionRepository.GetAll();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal(3, result[0].DispositivoId);
            Assert.Equal("Google", result[0].Dispositivo.Fabricante);
            Assert.Equal("Pixel 6", result[0].Dispositivo.Modelo);
            Assert.Equal("3.0.0", result[0].Version);
            Assert.Equal(0, result[0].Pendientes);
            Assert.Equal(2, result[1].DispositivoId);
            Assert.Equal("2.0.0", result[1].Version);
            Assert.Equal(0, result[1].Pendientes);
            Assert.Equal(1, result[2].DispositivoId);
            Assert.Equal("1.0.0", result[2].Version);
            Assert.Equal(1, result[2].Pendientes);
        }

        [Fact]
        public async Task CreateSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Apple', 'iPhone 13');
                """;
            command.ExecuteNonQuery();

            Actualizacion newActualizacion = new Actualizacion
            {
                DispositivoId = 1,
                Version = "1.0.0",
                Descripcion = "Primera actualización",
                Fecha = "2025-01-15 09:00:00"
            };

            //Act
            var result = await _actualizacionRepository.Create(newActualizacion);

            //Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task DeleteSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Apple', 'iPhone 13');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (1, '1.0.0', 'Primera actualización', '2025-01-15 09:00:00');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (1, '2.0.0', 'Segunda actualización', '2025-01-16 14:30:00');
                """;
            command.ExecuteNonQuery();

            //Act
            var result = await _actualizacionRepository.Delete(1);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteDispositivoCascadeSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Apple', 'iPhone 13');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Samsung', 'Galaxy S21');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (1, '1.0.0', 'Primera actualización', '2025-01-15 09:00:00');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (1, '2.0.0', 'Segunda actualización', '2025-01-16 14:30:00');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (2, '3.0.0', 'Tercera actualización', '2025-01-17 11:15:00');
                DELETE FROM dispositivos WHERE id = 1;
                """;
            command.ExecuteNonQuery();

            //Act
            var result = await _actualizacionRepository.GetAll();

            //Assert
            Assert.Single(result);
            Assert.Equal(2, result[0].DispositivoId);
            Assert.Equal("Samsung", result[0].Dispositivo.Fabricante);
        }

        [Fact]
        public async Task GetByIdSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Apple', 'iPhone 13');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (1, '1.0.0', 'Primera actualización', '2025-01-15 09:00:00');
                """;
            command.ExecuteNonQuery();

            //Act
            var result = await _actualizacionRepository.GetById(1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(1, result.DispositivoId);
            Assert.Equal("1.0.0", result.Version);
            Assert.Equal("Primera actualización", result.Descripcion);
            Assert.Equal("2025-01-15 09:00:00", result.Fecha);
            Assert.Equal("Apple", result.Dispositivo.Fabricante);
            Assert.Equal("iPhone 13", result.Dispositivo.Modelo);
        }

        [Fact]
        public async Task GetByDispositivoIdSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Apple', 'iPhone 13');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Samsung', 'Galaxy S21');
                INSERT INTO tecnicos (nombre, apellidos) VALUES ('Juan', 'García');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (1, '1.0.0', 'Primera actualización', '2025-01-15 09:00:00');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (1, '2.0.0', 'Segunda actualización', '2025-01-16 14:30:00');
                INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha) VALUES (2, '3.0.0', 'Tercera actualización', '2025-01-17 11:15:00');
                INSERT INTO verificaciones (actualizacion_id, tecnico_id, confirmado, fecha_conf) VALUES (1, 1, 0, NULL);
                """;
            command.ExecuteNonQuery();

            //Act
            var result = await _actualizacionRepository.GetByDispositivoId(1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, a => Assert.Equal(1, a.DispositivoId));
            Assert.All(result, a => Assert.Equal("Apple", a.Dispositivo.Fabricante));
            Assert.Equal("2.0.0", result[0].Version);
            Assert.Equal("1.0.0", result[1].Version);
            Assert.Equal(0, result[0].Pendientes);
            Assert.Equal(1, result[1].Pendientes);
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
