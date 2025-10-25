namespace Core.Models
{
    public class Tecnico
    {
        // init permite hacer el set solo en la inicialización del objeto
        public int Id { get; init; }
        public required string Nombre { get; set; }
        public required string Apellidos { get; set; }
        public string? Gaveta { get; set; }
        public string? NombrePC { get; set; }
        public string? UsuarioPC { get; set; }
    }
}
