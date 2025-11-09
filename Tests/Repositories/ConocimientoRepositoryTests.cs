using Core.Models;
using Data.Repositories;
using Microsoft.Data.Sqlite;

namespace Tests.Repositories
{
    public class ConocimientoRepositoryTests : IDisposable
    {
        private readonly ConocimientoRepository _conocimientoRepository;
        private readonly string _connectionString;
        private readonly SqliteConnection _connection;
        public ConocimientoRepositoryTests()
        {
            _connectionString = "Data Source=TestDB;Mode=Memory;Cache=Shared";
            _connection = new SqliteConnection(_connectionString);
            _connection.Open();
            InitializeDatabase();
            _conocimientoRepository = new ConocimientoRepository(_connection.ConnectionString);
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
                CREATE TABLE IF NOT EXISTS conocimientos (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    tecnico_id INTEGER NOT NULL,
                    dispositivo_id INTEGER NOT NULL,
                    FOREIGN KEY (tecnico_id) REFERENCES tecnicos(id) ON DELETE CASCADE,
                    FOREIGN KEY (dispositivo_id) REFERENCES dispositivos(id) ON DELETE CASCADE
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
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 3', 'Modelo 3');
                INSERT INTO conocimientos (tecnico_id, dispositivo_id) VALUES (1, 1);
                INSERT INTO conocimientos (tecnico_id, dispositivo_id) VALUES (1, 2);
                INSERT INTO conocimientos (tecnico_id, dispositivo_id) VALUES (2, 3);
                """;
            command.ExecuteNonQuery();

            //Act
            var result = await _conocimientoRepository.GetAll();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal(1, result[0].TecnicoId);
            Assert.Equal("James", result[0].Tecnico.Nombre);
            Assert.Equal(1, result[0].DispositivoId);
            Assert.Equal("Fabricante 1", result[0].Dispositivo.Fabricante);
            Assert.Equal(2, result[1].DispositivoId);
            Assert.Equal("Modelo 2", result[1].Dispositivo.Modelo);
            Assert.Equal(2, result[2].TecnicoId);
            Assert.Equal("Alice", result[2].Tecnico.Nombre);
        }

        [Fact]
        public async Task CreateSuccess()
        {
            //Arrange
                        using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc) VALUES ('James', 'Smith', 1, 'John-PC', 'john');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 1', 'Modelo 1');
                """;
            command.ExecuteNonQuery();

            Conocimiento newConocimiento = new Conocimiento
            {
                TecnicoId = 1,
                DispositivoId = 1
            };

            //Act
            var result = await _conocimientoRepository.Create(newConocimiento);

            //Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task DeleteSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc) VALUES ('James', 'Smith', 1, 'John-PC', 'john');
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc) VALUES ('Alice', 'Johnson', 2, 'Alice-PC', 'alice');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 1', 'Modelo 1');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 2', 'Modelo 2');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 3', 'Modelo 3');
                INSERT INTO conocimientos (tecnico_id, dispositivo_id) VALUES (1, 1);
                INSERT INTO conocimientos (tecnico_id, dispositivo_id) VALUES (1, 2);
                INSERT INTO conocimientos (tecnico_id, dispositivo_id) VALUES (2, 3);
                """;
            command.ExecuteNonQuery();

            //Act
            var result = await _conocimientoRepository.Delete(2);

            //Assert
            Assert.True(result);
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
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 2', 'Modelo 2');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 3', 'Modelo 3');
                INSERT INTO conocimientos (tecnico_id, dispositivo_id) VALUES (1, 1);
                INSERT INTO conocimientos (tecnico_id, dispositivo_id) VALUES (1, 2);
                INSERT INTO conocimientos (tecnico_id, dispositivo_id) VALUES (2, 3);
                DELETE FROM tecnicos WHERE id = 2;
                """;
            command.ExecuteNonQuery();

            //Act
            var result = await _conocimientoRepository.GetAll();

            //Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].TecnicoId);
            Assert.Equal("James", result[0].Tecnico.Nombre);
            Assert.Equal(1, result[0].DispositivoId);
            Assert.Equal("Fabricante 1", result[0].Dispositivo.Fabricante);
            Assert.Equal(2, result[1].DispositivoId);
            Assert.Equal("Modelo 2", result[1].Dispositivo.Modelo);
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
