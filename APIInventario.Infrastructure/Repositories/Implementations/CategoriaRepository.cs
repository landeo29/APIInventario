using APIInventario.Core.Models;
using APIInventario.Infrastructure.Data;
using APIInventario.Infrastructure.Repositories.Interfaces;
using APIInventario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace APIInventario.Infrastructure.Repositories.Implementations
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Categoria>> ObtenerTodasAsync()
        {
            return await _context.Categorias.ToListAsync();
        }

        public async Task<Categoria> ObtenerPorIdAsync(int id)
        {
            return await _context.Categorias.FindAsync(id);
        }

        public async Task AgregarAsync(Categoria categoria)
        {
            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var categoria = await ObtenerPorIdAsync(id);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
            }
        }
    }
}
