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
        private readonly PdfService _pdfService;


        public ProductosController(IProductoRepository productoRepo, PdfService pdfService)
        {
            _productoRepo = productoRepo;
            _pdfService = pdfService;
        }


        [HttpGet("listar")]
        [Authorize(Roles = "admin,empleado")]
        public async Task<IActionResult> ListarProductos()
        {
            var productos = await _productoRepo.ObtenerTodosAsync();
            if (productos == null || !productos.Any())
                return NotFound("¡No hay productos disponibles!");

            var productosDTO = productos.Select(p => new
            {
                p.Id,
                p.Nombre,
                p.Descripcion,
                p.Precio,
                p.Cantidad,
                p.CategoriaId,
                p.FechaCreacion,
                Categoria = p.Categoria != null ? p.Categoria.Nombre : "Sin categoría"
            });

            return Ok(productosDTO);
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
            producto.CategoriaId = productoActualizado.CategoriaId;

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

        [HttpPost("reportar-inventario")]
        [Authorize(Roles = "empleado")]
        public async Task<IActionResult> ReportarInventario([FromBody] ReporteInventarioDto reporte)
        {
            if (string.IsNullOrWhiteSpace(reporte.NumeroDestino) || string.IsNullOrWhiteSpace(reporte.Mensaje))
            {
                return BadRequest("Número de destino y mensaje son obligatorios.");
            }

            // LOG PARA VER LOS DATOS RECIBIDOS
            Console.WriteLine($"📩 Reporte recibido - Número: {reporte.NumeroDestino}, Mensaje: {reporte.Mensaje}");

            var payload = new
            {
                token = "slu4ht68lrcmijvs",  
                to = reporte.NumeroDestino,
                body = reporte.Mensaje,
                priority = 10
            };

            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync("https://api.ultramsg.com/instance110077/messages/chat", payload);
            var responseBody = await response.Content.ReadAsStringAsync(); 


            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Error al enviar el mensaje.");
            }

            return Ok("Mensaje enviado correctamente.");
        }


    }
}
