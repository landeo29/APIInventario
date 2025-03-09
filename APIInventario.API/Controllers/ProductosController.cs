using APIInventario.Infrastructure.Services;
using APIInventario.Core.Models;
using APIInventario.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIInventario.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoRepository _productoRepo;

        public ProductosController(IProductoRepository productoRepo)
        {
            _productoRepo = productoRepo;
        }

        [HttpGet("listar")]
        [Authorize(Roles = "admin,empleado")]
        public async Task<IActionResult> ListarProductos()
        {
            var productos = await _productoRepo.ObtenerTodosAsync();
            if (productos == null || !productos.Any())
                return NotFound("¡No hay productos disponibles!");

            return Ok(productos);
        }

        [HttpPost("crear")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CrearProducto([FromBody] Producto producto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos.");

            if (producto.Cantidad < 0)
                return BadRequest("La cantidad no puede ser negativa.");

            if (producto.Precio < 0)
                return BadRequest("El precio no puede ser negativo.");

            if (string.IsNullOrWhiteSpace(producto.Nombre))
                return BadRequest("El nombre del producto no puede estar vacío.");

            await _productoRepo.AgregarAsync(producto);
            return Ok("¡Producto creado con éxito!");
        }

        [HttpGet("obtener/{id}")]
        [Authorize(Roles = "admin,empleado")]
        public async Task<IActionResult> ObtenerProducto(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            var producto = await _productoRepo.ObtenerPorIdAsync(id);
            if (producto == null)
                return NotFound("¡Producto no encontrado!");

            return Ok(producto);
        }

        [HttpPut("actualizar/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ActualizarProducto(int id, [FromBody] Producto productoActualizado)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos.");

            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            if (productoActualizado.Cantidad < 0)
                return BadRequest("La cantidad no puede ser negativa.");

            if (productoActualizado.Precio < 0)
                return BadRequest("El precio no puede ser negativo.");

            if (string.IsNullOrWhiteSpace(productoActualizado.Nombre))
                return BadRequest("El nombre del producto no puede estar vacío.");

            var producto = await _productoRepo.ObtenerPorIdAsync(id);
            if (producto == null)
                return NotFound("¡Producto no encontrado!");

            producto.Nombre = productoActualizado.Nombre;
            producto.Descripcion = productoActualizado.Descripcion;
            producto.Precio = productoActualizado.Precio;
            producto.Cantidad = productoActualizado.Cantidad;

            /*if (producto.Cantidad < 5)
            {
                var emailService = HttpContext.RequestServices.GetService<EmailService>();
                var adminEmail = HttpContext.RequestServices.GetService<IConfiguration>().GetSection("EmailSettings:AdminEmail").Value;
                await emailService.EnviarCorreoAsync(adminEmail, "¡Inventario Bajo!", $"El producto {producto.Nombre} tiene menos de 5 unidades.");
            }*/


            await _productoRepo.ActualizarAsync(producto);
            return Ok("¡Producto actualizado con éxito!");
        }

        [HttpDelete("eliminar/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            var producto = await _productoRepo.ObtenerPorIdAsync(id);
            if (producto == null)
                return NotFound("¡Producto no encontrado!");

            await _productoRepo.EliminarAsync(id);
            return Ok("¡Producto eliminado con éxito!");
        }
    }
}
