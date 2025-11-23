namespace Core.Models
{
     public class Actualizacion
    {
        public int Id { get; set; } = 0;
        public int DispositivoId { get; set; } = 0;
        public Dispositivo Dispositivo { get; set; } = new();
        public string Version { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
        public int Pendientes { get; set; } = 0;
    }
}
