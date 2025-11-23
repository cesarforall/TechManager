using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Microsoft.Data.Sqlite;

namespace Data.Repositories
{
    public class VerificacionRepository : IVerificacionRepository
    {
        private readonly string _connectionString;

        public VerificacionRepository(string connectionString)
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
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception)
            {
                throw new RepositoryException("No se ha podido crear la tabla de verificaciones.");
            }
        }

        public async Task<int> Create(Verificacion verificacion)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = """
                    INSERT INTO verificaciones (actualizacion_id, tecnico_id, confirmado, fecha_conf)
                    VALUES ($actualizacion_id, $tecnico_id, $confirmado, $fecha_conf);
                    SELECT last_insert_rowid();
                    """;
                command.Parameters.AddWithValue("$actualizacion_id", verificacion.ActualizacionId);
                command.Parameters.AddWithValue("$tecnico_id", verificacion.TecnicoId);
                command.Parameters.AddWithValue("$confirmado", verificacion.Confirmado);
                command.Parameters.AddWithValue("$fecha_conf", verificacion.FechaConfirmacion);

                var result = await command.ExecuteScalarAsync();
                int newId = Convert.ToInt32(result);

                return newId > 0 ? newId : 0;
            }
            catch (Exception)
            {
                throw new RepositoryException("Error al añadir la verificación en la base de datos.");
            }
        }

        public async Task<bool> ConfirmVerification(int actualizacionId, int tecnicoId, string fechaConfirmacion)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = """
                    UPDATE verificaciones
                    SET confirmado = 1, fecha_conf = $fecha_conf
                    WHERE actualizacion_id = $actualizacion_id AND tecnico_id = $tecnico_id;
                    """;
                command.Parameters.AddWithValue("$actualizacion_id", actualizacionId);
                command.Parameters.AddWithValue("$tecnico_id", tecnicoId);
                command.Parameters.AddWithValue("$fecha_conf", fechaConfirmacion);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                throw new RepositoryException("Error al confirmar la verificación en la base de datos.");
            }
        }

        public async Task<List<Verificacion>> GetAll()
        {
            List<Verificacion> verificaciones = new List<Verificacion>();

            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT
                        v.id,
                        v.actualizacion_id,
                        v.tecnico_id,
                        v.confirmado,
                        v.fecha_conf,
                        t.nombre,
                        t.apellidos,
                        t.gaveta,
                        t.nombre_pc,
                        t.usuario_pc
                    FROM verificaciones v
                    LEFT JOIN tecnicos t ON v.tecnico_id = t.id;
                    """;
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    verificaciones.Add(
                        new Verificacion
                        {
                            Id = reader.GetInt32(0),
                            ActualizacionId = reader.GetInt32(1),
                            TecnicoId = reader.GetInt32(2),
                            Confirmado = reader.GetInt32(3),
                            FechaConfirmacion = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            Tecnico = new Tecnico
                            {
                                Id = reader.GetInt32(2),
                                Nombre = reader.GetString(5),
                                Apellidos = reader.GetString(6),
                                Gaveta = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                                NombrePC = reader.IsDBNull(8) ? null : reader.GetString(8),
                                UsuarioPC = reader.IsDBNull(9) ? null : reader.GetString(9),
                            }
                        }
                    );
                }

                return verificaciones;
            }
            catch (Exception)
            {
                throw new RepositoryException("Error al obtener la lista de verificaciones de la base de datos.");
            }
        }

        public async Task<Verificacion> GetById(int id)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT
                        v.id,
                        v.actualizacion_id,
                        v.tecnico_id,
                        v.confirmado,
                        v.fecha_conf,
                        t.nombre,
                        t.apellidos,
                        t.gaveta,
                        t.nombre_pc,
                        t.usuario_pc
                    FROM verificaciones v
                    LEFT JOIN tecnicos t ON v.tecnico_id = t.id
                    WHERE v.id = $id;
                    """;
                command.Parameters.AddWithValue("$id", id);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Verificacion
                    {
                        Id = reader.GetInt32(0),
                        ActualizacionId = reader.GetInt32(1),
                        TecnicoId = reader.GetInt32(2),
                        Confirmado = reader.GetInt32(3),
                        FechaConfirmacion = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                        Tecnico = new Tecnico
                        {
                            Id = reader.GetInt32(2),
                            Nombre = reader.GetString(5),
                            Apellidos = reader.GetString(6),
                            Gaveta = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                            NombrePC = reader.IsDBNull(8) ? null : reader.GetString(8),
                            UsuarioPC = reader.IsDBNull(9) ? null : reader.GetString(9),
                        }
                    };
                }
                else
                {
                    return new Verificacion();
                }
            }
            catch (Exception)
            {
                throw new RepositoryException("Error al obtener la verificación en la base de datos.");
            }
        }

        public async Task<List<Verificacion>> GetByActualizacionId(int actualizacionId)
        {
            List<Verificacion> verificaciones = new List<Verificacion>();

            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT
                        v.id,
                        v.actualizacion_id,
                        v.tecnico_id,
                        v.confirmado,
                        v.fecha_conf,
                        t.nombre,
                        t.apellidos,
                        t.gaveta,
                        t.nombre_pc,
                        t.usuario_pc
                    FROM verificaciones v
                    LEFT JOIN tecnicos t ON v.tecnico_id = t.id
                    WHERE v.actualizacion_id = $actualizacion_id;
                    """;
                command.Parameters.AddWithValue("$actualizacion_id", actualizacionId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    verificaciones.Add(
                        new Verificacion
                        {
                            Id = reader.GetInt32(0),
                            ActualizacionId = reader.GetInt32(1),
                            TecnicoId = reader.GetInt32(2),
                            Confirmado = reader.GetInt32(3),
                            FechaConfirmacion = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            Tecnico = new Tecnico
                            {
                                Id = reader.GetInt32(2),
                                Nombre = reader.GetString(5),
                                Apellidos = reader.GetString(6),
                                Gaveta = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                                NombrePC = reader.IsDBNull(8) ? null : reader.GetString(8),
                                UsuarioPC = reader.IsDBNull(9) ? null : reader.GetString(9),
                            }
                        }
                    );
                }

                return verificaciones;
            }
            catch (Exception)
            {
                throw new RepositoryException("Error al obtener las verificaciones por actualización en la base de datos.");
            }
        }
    }
}
