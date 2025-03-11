using APIInventario.Core.Models;
using APIInventario.Infrastructure.Data;
using APIInventario.Infrastructure.Repositories.Interfaces;
using APIInventario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace APIGestionInventarios.Infrastructure.Repositories.Implementations
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Producto >> ObtenerTodosAsync()
        {
            return await _context.Productos
            .Include(p => p.Categoria) 
            .ToListAsync();
        }

        public async Task<Producto> ObtenerPorIdAsync(int id)
        {
            return await _context.Productos.FindAsync(id);
        }

        public async Task AgregarAsync(Producto producto)
        {
            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Producto producto)
        {
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var producto = await ObtenerPorIdAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
            }
        }
    }
}
