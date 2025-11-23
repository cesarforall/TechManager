using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Microsoft.Data.Sqlite;

namespace Data.Repositories
{
    public class ActualizacionRepository : IActualizacionRepository
    {
        private readonly string _connectionString;

        public ActualizacionRepository(string connectionString)
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
                    CREATE TABLE IF NOT EXISTS actualizaciones (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        dispositivo_id INTEGER NOT NULL,
                        version TEXT NOT NULL,
                        descripcion TEXT NOT NULL,
                        fecha TEXT NOT NULL,
                        UNIQUE(dispositivo_id, version),
                        FOREIGN KEY (dispositivo_id) REFERENCES dispositivos(id) ON DELETE CASCADE
                    );
                    """;
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception)
            {
                return;
            }
        }

        public async Task<List<Actualizacion>> GetAll()
        {
            List<Actualizacion> actualizaciones = new List<Actualizacion>();

            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT
                        a.id,
                        a.dispositivo_id,
                        a.version,
                        a.descripcion,
                        a.fecha,
                        d.id as dispositivo_id,
                        d.fabricante,
                        d.modelo,
                        COUNT(CASE WHEN v.confirmado = 0 THEN 1 END) as pendientes
                    FROM actualizaciones a
                    INNER JOIN dispositivos d ON a.dispositivo_id = d.id
                    LEFT JOIN verificaciones v ON a.id = v.actualizacion_id
                    GROUP BY a.id
                    ORDER BY a.fecha DESC;
                    """;

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var actualizacion = new Actualizacion
                    {
                        Id = reader.GetInt32(0),
                        DispositivoId = reader.GetInt32(1),
                        Version = reader.GetString(2),
                        Descripcion = reader.GetString(3),
                        Fecha = reader.GetString(4),
                        Dispositivo = new Dispositivo
                        {
                            Id = reader.GetInt32(5),
                            Fabricante = reader.GetString(6),
                            Modelo = reader.GetString(7)
                        },
                        Pendientes = reader.GetInt32(8)
                    };

                    actualizaciones.Add(actualizacion);
                }

                return actualizaciones;
            }
            catch (Exception)
            {
                throw new RepositoryException($"Error al obtener las actualizaciones en la base de datos.");
            }
        }

        public async Task<Actualizacion> GetById(int id)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT
                        a.id,
                        a.dispositivo_id,
                        a.version,
                        a.descripcion,
                        a.fecha,
                        d.id as dispositivo_id,
                        d.fabricante,
                        d.modelo
                    FROM actualizaciones a
                    INNER JOIN dispositivos d ON a.dispositivo_id = d.id
                    WHERE a.id = $id;
                    """;
                command.Parameters.AddWithValue("$id", id);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Actualizacion
                    {
                        Id = reader.GetInt32(0),
                        DispositivoId = reader.GetInt32(1),
                        Version = reader.GetString(2),
                        Descripcion = reader.GetString(3),
                        Fecha = reader.GetString(4),
                        Dispositivo = new Dispositivo
                        {
                            Id = reader.GetInt32(5),
                            Fabricante = reader.GetString(6),
                            Modelo = reader.GetString(7)
                        }
                    };
                }
                else
                {
                    return new Actualizacion();
                }
            }
            catch (Exception)
            {
                throw new RepositoryException($"Error al obtener la actualización en la base de datos");
            }
        }

        public async Task<int> Create(Actualizacion actualizacion)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = """
                    INSERT INTO actualizaciones (dispositivo_id, version, descripcion, fecha)
                        VALUES ($dispositivo_id, $version, $descripcion, $fecha);
                    SELECT last_insert_rowid();
                    """;
                command.Parameters.AddWithValue("$dispositivo_id", actualizacion.DispositivoId);
                command.Parameters.AddWithValue("$version", actualizacion.Version);
                command.Parameters.AddWithValue("$descripcion", actualizacion.Descripcion);
                command.Parameters.AddWithValue("$fecha", actualizacion.Fecha);

                var id = await command.ExecuteScalarAsync();
                return Convert.ToInt32(id);
            }
            catch (Exception)
            {
                throw new RepositoryException($"Error al crear la actualización en la base de datos.");
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
                    DELETE FROM actualizaciones WHERE id = $id;
                    """;
                command.Parameters.AddWithValue("$id", id);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                throw new RepositoryException($"Error al eliminar la actualización en la base de datos.");
            }
        }

        public async Task<List<Actualizacion>> GetByDispositivoId(int dispositivoId)
        {
            List<Actualizacion> actualizaciones = new List<Actualizacion>();

            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT
                        a.id,
                        a.dispositivo_id,
                        a.version,
                        a.descripcion,
                        a.fecha,
                        d.id as dispositivo_id,
                        d.fabricante,
                        d.modelo,
                        COUNT(CASE WHEN v.confirmado = 0 THEN 1 END) as pendientes
                    FROM actualizaciones a
                    INNER JOIN dispositivos d ON a.dispositivo_id = d.id
                    LEFT JOIN verificaciones v ON a.id = v.actualizacion_id
                    WHERE a.dispositivo_id = $dispositivo_id
                    GROUP BY a.id
                    ORDER BY a.fecha DESC;
                    """;
                command.Parameters.AddWithValue("$dispositivo_id", dispositivoId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var actualizacion = new Actualizacion
                    {
                        Id = reader.GetInt32(0),
                        DispositivoId = reader.GetInt32(1),
                        Version = reader.GetString(2),
                        Descripcion = reader.GetString(3),
                        Fecha = reader.GetString(4),
                        Dispositivo = new Dispositivo
                        {
                            Id = reader.GetInt32(5),
                            Fabricante = reader.GetString(6),
                            Modelo = reader.GetString(7)
                        },
                        Pendientes = reader.GetInt32(8)
                    };

                    actualizaciones.Add(actualizacion);
                }

                return actualizaciones;
            }
            catch (Exception)
            {
                throw new RepositoryException($"Error al obtener las actualizaciones por dispositivo en la base de datos.");
            }
        }
    }
}
