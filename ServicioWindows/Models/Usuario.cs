namespace ServicioWindows.Models
{
    public class Usuario
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string ApellidoP { get; set; }
        public string ApellidoM { get; set; }
        public string Correo { get; set; }
        public string FechaNac { get; set; }
        public string FechaRegistro { get; set; }
        public bool Activo { get; set; }
        public string FechaMod { get; set; }
    }
}
