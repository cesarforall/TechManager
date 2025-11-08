using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Microsoft.Data.Sqlite;

namespace Data.Repositories
{
    public class DispositivoRepository : IDispositivoRepository
    {
        private readonly string _connectionString;
        public DispositivoRepository(string connectionString)
        {
            _connectionString = connectionString;
            initialize().Wait();
        }

        private async Task initialize()
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = """
                    CREATE TABLE IF NOT EXISTS dispositivos (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        fabricante TEXT NOT NULL,
                        modelo TEXT NOT NULL,
                        UNIQUE (fabricante, modelo)
                    );
                    """;
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception)
            {
                return;
            }
        }

        public async Task<int?> Create(Dispositivo dispositivo)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = """
                    INSERT INTO dispositivos (fabricante, modelo) VALUES ($fabricante, $modelo);
                    SELECT last_insert_rowid();
                    """;
                command.Parameters.AddWithValue("$fabricante", dispositivo.Fabricante);
                command.Parameters.AddWithValue("$modelo", dispositivo.Modelo);

                var id = await command.ExecuteScalarAsync();
                return Convert.ToInt32(id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = """
                    DELETE FROM dispositivos WHERE id = $id;
                    """;
                command.Parameters.AddWithValue("$id", id);
                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Dispositivo>?> GetAll()
        {
            List<Dispositivo> dispositivos = new List<Dispositivo>();

            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT id, fabricante, modelo FROM dispositivos;
                    """;
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var dispositivo = new Dispositivo
                    {
                        Id = reader.GetInt32(0),
                        Fabricante = reader.GetString(1),
                        Modelo = reader.GetString(2)
                    };

                    dispositivos.Add(dispositivo);
                }

                return dispositivos;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Dispositivo?> GetById(int id)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = """
                        SELECT id, fabricante, modelo FROM dispositivos WHERE id = $id;
                    """;
                command.Parameters.AddWithValue("$id", id);
                var result = await command.ExecuteReaderAsync();

                if (await result.ReadAsync())
                {
                    return new Dispositivo
                    {
                        Id = result.GetInt32(0),
                        Fabricante = result.GetString(1),
                        Modelo = result.GetString(2)
                    };
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> Update(Dispositivo dispositivo)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = """
                    UPDATE dispositivos
                    SET fabricante = $fabricante,
                        modelo = $modelo
                    WHERE id = $id;
                    """;
                command.Parameters.AddWithValue("$id", dispositivo.Id);
                command.Parameters.AddWithValue("$fabricante", dispositivo.Fabricante);
                command.Parameters.AddWithValue("$modelo", dispositivo.Modelo);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Dispositivo?> GetByFabricanteModelo(string fabricante, string modelo)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT id, fabricante, modelo FROM dispositivos WHERE fabricante = $fabricante AND modelo = $modelo;
                    """;
                command.Parameters.AddWithValue("$fabricante", fabricante);
                command.Parameters.AddWithValue("$modelo", modelo);

                var result = await command.ExecuteReaderAsync();

                if (await result.ReadAsync())
                {
                    return new Dispositivo
                    {
                        Id = result.GetInt32(0),
                        Fabricante = result.GetString(1),
                        Modelo = result.GetString(2)
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Error al obtener el dispositivo por fabricante y modelo: {ex.Message}");
            }
        }
    }
}
