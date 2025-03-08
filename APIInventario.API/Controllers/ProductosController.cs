using APIInventario.Core.Interfaces;
using APIInventario.Core.Models;
using APIInventario.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APIInventario.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoRepository _productoRepo;

        public ProductosController(IProductoRepository productoRepo)
        {
            _productoRepo = productoRepo;
        }

        [HttpGet("listar")]
        public async Task<IActionResult> ListarProductos()
        {
            var productos = await _productoRepo.ObtenerTodosAsync();
            if (!productos.Any())
                return NotFound("No hay productos registrados.");

            return Ok(productos);
        }

        [HttpPost("crear")]
        public async Task<IActionResult> CrearProducto([FromBody] Producto producto)
        {
            if (string.IsNullOrWhiteSpace(producto.Nombre) || producto.Precio <= 0)
                return BadRequest("Datos inválidos para el producto.");

            await _productoRepo.AgregarAsync(producto);
            return Ok("Producto creado con éxito!");
        }

        [HttpGet("obtener/{id}")]
        public async Task<IActionResult> ObtenerProducto(int id)
        {
            var producto = await _productoRepo.ObtenerPorIdAsync(id);
            if (producto == null)
                return NotFound("Producto no encontrado.");

            return Ok(producto);
        }

        [HttpPut("actualizar/{id}")]
        public async Task<IActionResult> ActualizarProducto(int id, [FromBody] Producto productoActualizado)
        {
            var producto = await _productoRepo.ObtenerPorIdAsync(id);
            if (producto == null)
                return NotFound("Producto no encontrado.");

            producto.Nombre = productoActualizado.Nombre;
            producto.Descripcion = productoActualizado.Descripcion;
            producto.Precio = productoActualizado.Precio;
            producto.Cantidad = productoActualizado.Cantidad;
            producto.CategoriaId = productoActualizado.CategoriaId;

            await _productoRepo.ActualizarAsync(producto);
            return Ok("Producto actualizado con éxito!");
        }

        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto = await _productoRepo.ObtenerPorIdAsync(id);
            if (producto == null)
                return NotFound("Producto no encontrado.");

            await _productoRepo.EliminarAsync(id);
            return Ok("Producto eliminado con éxito!");
        }
    }
}
