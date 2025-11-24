using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Microsoft.Data.Sqlite;

namespace Data.Repositories
{
    public class ConocimientoRepository : IConocimientoRepository
    {
        private readonly string _connectionString;

        public ConocimientoRepository(string connectionString)
        {
            _connectionString = connectionString;
            Initialize().Wait();
        }

        private async Task Initialize()
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = """
                    CREATE TABLE IF NOT EXISTS conocimientos (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        tecnico_id INTEGER NOT NULL,
                        dispositivo_id INTEGER NOT NULL,
                        UNIQUE (tecnico_id, dispositivo_id),
                        FOREIGN KEY (tecnico_id) REFERENCES tecnicos(id) ON DELETE CASCADE,
                        FOREIGN KEY (dispositivo_id) REFERENCES dispositivos(id) ON DELETE CASCADE
                    );
                    """;
                await command.ExecuteNonQueryAsync();

            }
            catch (Exception)
            {
                throw new RepositoryException("No se ha podido crear la tabla de conocimientos.");
            }
        }

        public async Task<int> Create(Conocimiento conocimiento)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = """
                    INSERT INTO conocimientos (tecnico_id, dispositivo_id) VALUES ($tecnico_id, $dispositivo_id);
                    SELECT last_insert_rowid();
                    """;
                command.Parameters.AddWithValue("$tecnico_id", conocimiento.TecnicoId);
                command.Parameters.AddWithValue("$dispositivo_id", conocimiento.DispositivoId);

                var result = await command.ExecuteScalarAsync();
                int newId = Convert.ToInt32(result);

                return newId > 0 ? newId : 0;
            }
            catch (Exception)
            {
                throw new RepositoryException("Error al añadir el conocimiento en la base de datos.");
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
                    DELETE FROM conocimientos WHERE id = $id;
                    """;
                command.Parameters.AddWithValue("$id", id);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                throw new RepositoryException("Error al eliminar el conocimiento de la base de datos.");
            }
        }

        public async Task<List<Conocimiento>> GetAll()
        {
            List<Conocimiento> conocimientos = new List<Conocimiento>();

            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT
                        c.id AS conocimiento_id,
                        c.tecnico_id AS conocimiento_tecnico_id,
                        c.dispositivo_id AS conocimiento_dispositivo_id,
                        t.id AS tecnico_id,
                        t.nombre,
                        t.apellidos,
                        t.gaveta,
                        t.nombre_pc,
                        t.usuario_pc,
                        d.id AS dispositivo_id,
                        d.fabricante,
                        d.modelo
                    FROM conocimientos c
                    LEFT JOIN tecnicos t ON c.tecnico_id = t.id
                    LEFT JOIN dispositivos d ON c.dispositivo_id = d.id;                    
                    """;
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    conocimientos.Add(
                        new Conocimiento
                        {
                            Id = reader.GetInt32(0),
                            TecnicoId = reader.GetInt32(1),
                            DispositivoId = reader.GetInt32(2),
                            Tecnico = new Tecnico
                            {
                                Id = reader.GetInt32(3),
                                Nombre = reader.GetString(4),
                                Apellidos = reader.GetString(5),
                                Gaveta = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                                NombrePC = reader.IsDBNull(7) ? null : reader.GetString(7),
                                UsuarioPC = reader.IsDBNull(8) ? null : reader.GetString(8),
                            },
                            Dispositivo = new Dispositivo
                            {
                                Id = reader.GetInt32(9),
                                Fabricante = reader.GetString(10),
                                Modelo = reader.GetString(11)
                            }
                        }
                        );
                }

                return conocimientos;
            }
            catch (Exception)
            {
                throw new RepositoryException("Error al obtener la lista de conocimientos de la base de datos.");
            }
        }

        public async Task<Conocimiento> GetById(int id)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT
                        c.id AS conocimiento_id,
                        c.tecnico_id AS conocimiento_tecnico_id,
                        c.dispositivo_id AS conocimiento_dispositivo_id,
                        t.id AS tecnico_id,
                        t.nombre,
                        t.apellidos,
                        t.gaveta,
                        t.nombre_pc,
                        t.usuario_pc,
                        d.id AS dispositivo_id,
                        d.fabricante,
                        d.modelo
                    FROM conocimientos c
                    LEFT JOIN tecnicos t ON c.tecnico_id = t.id
                    LEFT JOIN dispositivos d ON c.dispositivo_id = d.id
                    WHERE c.id = $id;
                    """;
                command.Parameters.AddWithValue("$id", id);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Conocimiento
                    {
                        Id = reader.GetInt32(0),
                        TecnicoId = reader.GetInt32(1),
                        DispositivoId = reader.GetInt32(2),
                        Tecnico = new Tecnico
                        {
                            Id = reader.GetInt32(3),
                            Nombre = reader.GetString(4),
                            Apellidos = reader.GetString(5),
                            Gaveta = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                            NombrePC = reader.IsDBNull(7) ? null : reader.GetString(7),
                            UsuarioPC = reader.IsDBNull(8) ? null : reader.GetString(8),
                        },
                        Dispositivo = new Dispositivo
                        {
                            Id = reader.GetInt32(9),
                            Fabricante = reader.GetString(10),
                            Modelo = reader.GetString(11)
                        }
                    };
                }
                else
                {
                    return new Conocimiento();
                }
            }
            catch (Exception)
            {
                throw new RepositoryException("Error al obtener el conocimiento en la base de datos.");
            }
        }

        public async Task<List<Conocimiento>> GetByDispositivoId(int dispositivoId)
        {
            List<Conocimiento> conocimientos = new List<Conocimiento>();

            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT
                        c.id AS conocimiento_id,
                        c.tecnico_id AS conocimiento_tecnico_id,
                        c.dispositivo_id AS conocimiento_dispositivo_id,
                        t.id AS tecnico_id,
                        t.nombre,
                        t.apellidos,
                        t.gaveta,
                        t.nombre_pc,
                        t.usuario_pc,
                        d.id AS dispositivo_id,
                        d.fabricante,
                        d.modelo
                    FROM conocimientos c
                    LEFT JOIN tecnicos t ON c.tecnico_id = t.id
                    LEFT JOIN dispositivos d ON c.dispositivo_id = d.id
                    WHERE c.dispositivo_id = $dispositivo_id;
                    """;
                command.Parameters.AddWithValue("$dispositivo_id", dispositivoId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    conocimientos.Add(
                        new Conocimiento
                        {
                            Id = reader.GetInt32(0),
                            TecnicoId = reader.GetInt32(1),
                            DispositivoId = reader.GetInt32(2),
                            Tecnico = new Tecnico
                            {
                                Id = reader.GetInt32(3),
                                Nombre = reader.GetString(4),
                                Apellidos = reader.GetString(5),
                                Gaveta = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                                NombrePC = reader.IsDBNull(7) ? null : reader.GetString(7),
                                UsuarioPC = reader.IsDBNull(8) ? null : reader.GetString(8),
                            },
                            Dispositivo = new Dispositivo
                            {
                                Id = reader.GetInt32(9),
                                Fabricante = reader.GetString(10),
                                Modelo = reader.GetString(11)
                            }
                        }
                    );
                }

                return conocimientos;
            }
            catch (Exception)
            {
                throw new RepositoryException("Error al obtener los conocimientos por dispositivo en la base de datos.");
            }
        }
    }
}
