using Microsoft.Data.Sqlite;
using Core.Interfaces;
using Core.Models;

namespace Data
{
    public class TecnicoRepository : ITecnicoRepository
    {
        private readonly string _connectionString;

        public TecnicoRepository(string connectionString)
        {
            _connectionString = connectionString;
            initialize().Wait();
        }

        private async Task initialize()
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
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
            await command.ExecuteNonQueryAsync();
        }

        public async Task<int> create(Tecnico tecnico)
        {
            var connection  = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO tecnicos (nombre, apellidos, gaveta, nombre_pc, usuario_pc)
                VALUES ($nombre, $apellidos, $gaveta, $nombre_pc, $usuario_pc);
                SELECT last_insert_rowid();
            """;
            command.Parameters.AddWithValue("$nombre", tecnico.Nombre);
            command.Parameters.AddWithValue("$apellidos", tecnico.Apellidos);
            // Si la propiedad es null, se asigna DBNull.Value para almacenar NULL en la base de datos
            command.Parameters.AddWithValue("$gaveta", tecnico.Gaveta ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("$nombre_pc", tecnico.NombrePC ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("$usuario_pc", tecnico.UsuarioPC ?? (object)DBNull.Value);

            var id = await command.ExecuteScalarAsync();
            return Convert.ToInt32(id);
        }

        public async Task<bool> delete(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            command.CommandText = """
                DELETE FROM tecnicos
                WHERE id = $id
            """;
            command.Parameters.AddWithValue("$id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<List<Tecnico>> getAll()
        {
            var tecnicos = new List<Tecnico>();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT id, nombre, apellidos, gaveta, nombre_pc, usuario_pc
                FROM tecnicos
            """;

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var tecnico = new Tecnico
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Apellidos = reader.GetString(2),
                    // Asignación de null a la propiedad que se ha almacenado como NULL en la base de datos
                    Gaveta = reader.IsDBNull(3) ? null : reader.GetString(3),
                    NombrePC = reader.IsDBNull(4) ? null : reader.GetString(4),
                    UsuarioPC = reader.IsDBNull(5) ? null : reader.GetString(5)
                };
                tecnicos.Add(tecnico);
            }

            return tecnicos;
        }

        public async Task<Tecnico?> getById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT id, nombre, apellidos, gaveta, nombre_pc, usuario_pc
                FROM tecnicos
                WHERE id = $id
            """;
            command.Parameters.AddWithValue("$id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Tecnico
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Apellidos = reader.GetString(2),                    
                    Gaveta = reader.IsDBNull(3) ? null : reader.GetString(3),
                    NombrePC = reader.IsDBNull(4) ? null : reader.GetString(4),
                    UsuarioPC = reader.IsDBNull(5) ? null : reader.GetString(5)
                };
            }

            return null;
        }

        public async Task<bool> update(Tecnico tecnico)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            command.CommandText = """
                UPDATE tecnicos
                SET nombre = $nombre,
                    apellidos = $apellidos,
                    gaveta = $gaveta,
                    nombre_pc = $nombre_pc,
                    usuario_pc = $usuario_pc
                WHERE id = $id
            """;
            command.Parameters.AddWithValue("$id", tecnico.Id);
            command.Parameters.AddWithValue("$nombre", tecnico.Nombre);
            command.Parameters.AddWithValue("$apellidos", tecnico.Apellidos);
            command.Parameters.AddWithValue("$gaveta", tecnico.Gaveta ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("$nombre_pc", tecnico.NombrePC ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("$usuario_pc", tecnico.UsuarioPC ?? (object)DBNull.Value);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }
    }
}
