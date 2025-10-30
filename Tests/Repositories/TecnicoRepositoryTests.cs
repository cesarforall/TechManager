using Data;
using Core.Models;
using Microsoft.Data.Sqlite;
using Core.Exceptions;

namespace Tests.Repositories
{
    public class TecnicoRepositoryTests : IDisposable
    {
        private readonly string _connectionString;
        private readonly SqliteConnection _connection;
        private readonly TecnicoRepository _tecnicoRepository;
        public TecnicoRepositoryTests()
        {
            // Usar una base de datos compartida en memoria
            _connectionString = "Data Source=TestDB;Mode=Memory;Cache=Shared";
            _connection = new SqliteConnection(_connectionString);
            _connection.Open();
            CreateDatabase();
            _tecnicoRepository = new TecnicoRepository(_connection.ConnectionString);
        }

        internal void CreateDatabase()
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
            """;
            command.ExecuteNonQuery();
        }

        // Limpia los recursos al finalizar las pruebas
        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }

        [Fact]
        public async void GetAllSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc)
                VALUES ('John', 'Doe', 1, 'John-PC', 'john'),
                       ('Jane', 'Smith', 2, 'Jane-PC', 'jane');
            """;
            command.ExecuteNonQuery();

            //Act
            var tecnicos = await _tecnicoRepository.getAll();

            //Assert
            Assert.Equal(2, tecnicos.Count);
            Assert.Contains(tecnicos, t => t.Id == 1 && t.Nombre == "John");
            Assert.Contains(tecnicos, t => t.Id == 2 && t.Nombre == "Jane");
        }

        [Fact]
        public async void GetByNombrePCSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc)
                VALUES ('John', 'Doe', 1, 'John-PC', 'john')
            """;
            command.ExecuteNonQuery();

            //Act
            var tecnico = await _tecnicoRepository.getByNombrePC("John-PC");

            //Assert
            Assert.NotNull(tecnico);
            Assert.Equal(1, tecnico.Id);
        }

        [Fact]
        public async void CreateSuccess()
        {
            //Arrange
            var tecnicoToCreate = new Tecnico
            {
                Nombre = "Alice",
                Apellidos = "Johnson",
                Gaveta = 3,
                NombrePC = "Alice-PC",
                UsuarioPC = "alice"
            };

            //Act
            int createdTecnico = await _tecnicoRepository.create(tecnicoToCreate);

            //Assert
            Assert.Equal(1, createdTecnico);
        }

        [Fact]
        public async void UpdateInvalidIdFail()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc)
                VALUES ('James', 'Smith', 1, 'John-PC', 'john');
            """;
            command.ExecuteNonQuery();

            var tecnicoToUpdate = new Tecnico
            {
                Id = 999,
                Nombre = "James",
                Apellidos = "Smith",
                Gaveta = 2,
                NombrePC = "John-PC",
                UsuarioPC = "john"
            };

            //Act
            var tecnico = await _tecnicoRepository.update(tecnicoToUpdate);

            //Assert
            Assert.False(tecnico);
        }
        [Fact]
        public async void UpdateGavetaAlreadyAssignedFail()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc)
                VALUES ('James', 'Smith', 1, 'John-PC', 'john'), ('Alice', 'Johnson', 2, 'Alice-PC', 'alice');
            """;
            command.ExecuteNonQuery();

            var tecnicoToUpdate = new Tecnico
            {
                Id = 1,
                Nombre = "James",
                Apellidos = "Smith",
                Gaveta = 2,
                NombrePC = "John-PC",
                UsuarioPC = "john"
            };

            //Act & Assert
            await Assert.ThrowsAsync<RepositoryException>(
                async () => await _tecnicoRepository.update(tecnicoToUpdate)
            );
        }

        [Fact]
        public async void UpdateSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc)
                VALUES ('James', 'Smith', 1, 'John-PC', 'john');
            """;
            command.ExecuteNonQuery();

            var tecnicoToUpdate = new Tecnico
            {
                Id = 1,
                Nombre = "James",
                Apellidos = "Smith",
                Gaveta = 2,
                NombrePC = "John-PC",
                UsuarioPC = "john"
            };

            //Act
            var tecnico = await _tecnicoRepository.update(tecnicoToUpdate);

            //Assert
            Assert.True(tecnico);
        }

        [Fact]
        public async void DeleteFail()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc)
                VALUES ('James', 'Smith', 1, 'John-PC', 'john');
            """;
            command.ExecuteNonQuery();

            // Act
            bool rowsAffected = await _tecnicoRepository.delete(2);

            //Assert
            Assert.False(rowsAffected);
        }
    }
}
