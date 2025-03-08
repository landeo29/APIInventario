using APIInventario.Core.Models;

namespace APIInventario.Infrastructure.Repositories.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> ObtenerTodasAsync();
        Task<Categoria> ObtenerPorIdAsync(int id);
        Task AgregarAsync(Categoria categoria);
        Task ActualizarAsync(Categoria categoria);
        Task EliminarAsync(int id);
    }
}
