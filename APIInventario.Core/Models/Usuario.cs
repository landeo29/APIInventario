namespace APIInventario.Core.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = "empleado";
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
