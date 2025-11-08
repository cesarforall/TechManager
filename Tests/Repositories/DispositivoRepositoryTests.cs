using Core.Models;
using Data.Repositories;
using Microsoft.Data.Sqlite;

namespace Tests.Repositories
{
    public class DispositivoRepositoryTests : IDisposable
    {
        private readonly string _connectionString;
        private readonly SqliteConnection _connection;
        private readonly DispositivoRepository _dispositivoRepository;
        public DispositivoRepositoryTests()
        {
            _connectionString = "Data Source=TestDB;Mode=Memory;Cache=Shared";
            _connection = new SqliteConnection(_connectionString);
            _connection.Open();
            initializeDatabase();
            _dispositivoRepository = new DispositivoRepository(_connection.ConnectionString);
        }

        private void initializeDatabase()
        {
            using var command = _connection.CreateCommand();
            command.CommandText = """
                CREATE TABLE IF NOT EXISTS dispositivos (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    fabricante TEXT NOT NULL,
                    modelo TEXT NOT NULL,
                    UNIQUE (fabricante, modelo)
                );
                """;
            command.ExecuteNonQuery();
        }
        
        [Fact]
        public async Task CreateSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 1', 'Modelo 1');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 2', 'Modelo 2');
                """;
            command.ExecuteNonQuery();

            Dispositivo dispositivo = new Dispositivo()
            {
                Fabricante = "Fabricante 3",
                Modelo = "Modelo 3"
            };

            //Act
            var id = await _dispositivoRepository.Create(dispositivo);

            //Assert
            Assert.Equal(3, Convert.ToInt32(id));
        }

        [Fact]
        public async void DeleteSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 1', 'Modelo 1');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 2', 'Modelo 2');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 3', 'Modelo 3');
                """;
            command.ExecuteNonQuery();

            //Act
            var result = await _dispositivoRepository.Delete(2);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async void GetAllSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 1', 'Modelo 1');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 2', 'Modelo 2');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 3', 'Modelo 3');
                """;
            command.ExecuteNonQuery();

            //Act
            var result = await _dispositivoRepository.GetAll();

            //Assert
            Assert.Equal(3, result?.Count());
        }

        [Fact]
        public async void GetByIdSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 1', 'Modelo 1');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 2', 'Modelo 2');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 3', 'Modelo 3');
                """;
            command.ExecuteNonQuery();

            //Act
            var tecnico = await _dispositivoRepository.GetById(2);

            //Assert
            Assert.Equal("Fabricante 2", tecnico?.Fabricante);
            Assert.Equal("Modelo 2", tecnico?.Modelo);
        }

        [Fact]
        public async void UpdateSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 1', 'Modelo 1');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 2', 'Modelo 2');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 3', 'Modelo 3');
                """;
            command.ExecuteNonQuery();

            Dispositivo dispositivo = new Dispositivo
            {
                Id = 2,
                Fabricante = "Fabricante 4",
                Modelo = "Modelo 4"
            };

            //Act
            var result = await _dispositivoRepository.Update(dispositivo);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async void GetByIdFabricanteModeloSuccess()
        {
            //Arrange
            using var command = _connection.CreateCommand();
            command.CommandText = """
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 1', 'Modelo 1');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 2', 'Modelo 2');
                INSERT INTO dispositivos (fabricante, modelo) VALUES ('Fabricante 3', 'Modelo 3');
                """;
            command.ExecuteNonQuery();

            //Act
            var tecnico = await _dispositivoRepository.GetByFabricanteModelo("Fabricante 2", "Modelo 2");

            //Assert
            Assert.Equal("Fabricante 2", tecnico?.Fabricante);
            Assert.Equal("Modelo 2", tecnico?.Modelo);
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
