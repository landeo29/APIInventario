using APIInventario.Core.Interfaces;
using APIInventario.Core.Models;
using APIInventario.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APIInventario.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepository _categoriaRepo;

        public CategoriasController(ICategoriaRepository categoriaRepo)
        {
            _categoriaRepo = categoriaRepo;
        }

        [HttpGet("listar")]
        public async Task<IActionResult> ListarCategorias()
        {
            var categorias = await _categoriaRepo.ObtenerTodasAsync();
            if (!categorias.Any())
                return NotFound("No hay categorías registradas.");

            return Ok(categorias);
        }

        [HttpPost("crear")]
        public async Task<IActionResult> CrearCategoria([FromBody] Categoria categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria.Nombre))
                return BadRequest("El nombre de la categoría es requerido.");

            await _categoriaRepo.AgregarAsync(categoria);
            return Ok("Categoría creada con éxito!");
        }

        [HttpGet("obtener/{id}")]
        public async Task<IActionResult> ObtenerCategoria(int id)
        {
            var categoria = await _categoriaRepo.ObtenerPorIdAsync(id);
            if (categoria == null)
                return NotFound("Categoría no encontrada.");

            return Ok(categoria);
        }

        [HttpPut("actualizar/{id}")]
        public async Task<IActionResult> ActualizarCategoria(int id, [FromBody] Categoria categoriaActualizada)
        {
            var categoria = await _categoriaRepo.ObtenerPorIdAsync(id);
            if (categoria == null)
                return NotFound("Categoría no encontrada.");

            if (string.IsNullOrWhiteSpace(categoriaActualizada.Nombre))
                return BadRequest("El nombre de la categoría es requerido.");

            categoria.Nombre = categoriaActualizada.Nombre;
            await _categoriaRepo.ActualizarAsync(categoria);

            return Ok("Categoría actualizada con éxito!");
        }

        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            var categoria = await _categoriaRepo.ObtenerPorIdAsync(id);
            if (categoria == null)
                return NotFound("Categoría no encontrada.");

            await _categoriaRepo.EliminarAsync(id);
            return Ok("Categoría eliminada con éxito!");
        }
    }
}
