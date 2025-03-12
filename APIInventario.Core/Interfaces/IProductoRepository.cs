using APIInventario.Core.Models;

namespace APIInventario.Infrastructure.Repositories.Interfaces
{
    public interface IProductoRepository
    {
        Task<IEnumerable<Producto>> ObtenerTodosAsync();
        Task<Producto> ObtenerPorIdAsync(int id);
        Task AgregarAsync(Producto producto);
        Task ActualizarAsync(Producto producto);
        Task EliminarAsync(int id);
        Task<List<Producto>> ObtenerProductosConStockBajo();

    }
}
