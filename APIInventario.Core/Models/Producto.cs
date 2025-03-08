using System.Text.Json.Serialization;

namespace APIInventario.Core.Models
{
    public class Producto
    {
        public int Id { get; set; }                         
        public string Nombre { get; set; } = null!;           
        public string Descripcion { get; set; } = null!;     
        public decimal Precio { get; set; }                  
        public int Cantidad { get; set; }                    
        public DateTime FechaCreacion { get; set; }          

        public int CategoriaId { get; set; }
        [JsonIgnore]
        public Categoria? Categoria { get; set; }

    }
}
