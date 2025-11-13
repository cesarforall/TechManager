namespace Core.Models
{
    public class Conocimiento
    {
        public int Id { get; set; }
        public int TecnicoId { get; set; }
        public Tecnico Tecnico { get; set; } = new();
        public int DispositivoId { get; set; }
        public Dispositivo Dispositivo { get; set; } = new();
        public bool IsChecked { get; set; } = false;
    }
}
