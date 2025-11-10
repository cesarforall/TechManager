namespace Core.Models
{
    public class Dispositivo
    {
        public int? Id { get; set; }
        public string? Fabricante { get; set; }
        public string? Modelo { get; set; }
        public string FullName => $"{Fabricante} {Modelo}";
    }
}
