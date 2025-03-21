﻿namespace APIInventario.Core.Models
{
    public class Categoria
    {
        public int Id { get; set; }                  
        public string Nombre { get; set; } = null!;   

        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
