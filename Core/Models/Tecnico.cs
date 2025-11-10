namespace Core.Models
{
    public class Tecnico
    {
        public int Id { get; init; }
        public string? Nombre { get; set; }
        public string? Apellidos { get; set; }
        public int? Gaveta { get; set; }
        public string? NombrePC { get; set; }
        public string? UsuarioPC { get; set; }
        public string FullName => $"{Nombre} {Apellidos}";
    }
}
