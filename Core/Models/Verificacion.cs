namespace Core.Models
{
    public class Verificacion
    {
        public int Id { get; set; } = 0;
        public int ActualizacionId { get; set; } = 0;
        public int TecnicoId { get; set; } = 0;
        public Tecnico Tecnico { get; set; } = new();
        public int Confirmado { get; set; } = 0;
        public string FechaConfirmacion { get; set; } = string.Empty;
    }
}
