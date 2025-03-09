using APIInventario.Core.Interfaces;
using APIInventario.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIINVENTARIO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepo;

        public UsuariosController(IUsuarioRepository usuarioRepo)
        {
            _usuarioRepo = usuarioRepo;
        }

        [HttpGet("listar")]
        public async Task<IActionResult> ListarUsuarios()
        {
            var usuarios = await _usuarioRepo.ObtenerTodosAsync();
            if (usuarios == null || !usuarios.Any())
                return NotFound("No hay usuarios registrados.");
            return Ok(usuarios);
        }

        [HttpPost("crear")]
        public async Task<IActionResult> CrearUsuario([FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos.");

            if (string.IsNullOrWhiteSpace(usuario.Username) || string.IsNullOrWhiteSpace(usuario.Password))
                return BadRequest("El nombre de usuario y la contraseña son obligatorios.");

            var existeUsuario = await _usuarioRepo.ObtenerPorNombreAsync(usuario.Username);
            if (existeUsuario != null)
                return Conflict("El nombre de usuario ya está en uso.");

            await _usuarioRepo.AgregarAsync(usuario);
            return Ok("Usuario creado con éxito!");
        }

        [HttpGet("obtener/{id}")]
        public async Task<IActionResult> ObtenerUsuario(int id)
        {
            if (id <= 0)
                return BadRequest("ID inválido.");

            var usuario = await _usuarioRepo.ObtenerPorIdAsync(id);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            return Ok(usuario);
        }

        [HttpPut("actualizar/{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] Usuario usuarioActualizado)
        {
            if (id <= 0)
                return BadRequest("ID inválido.");

            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos.");

            var usuario = await _usuarioRepo.ObtenerPorIdAsync(id);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            usuario.Username = usuarioActualizado.Username;
            usuario.Password = usuarioActualizado.Password;
            usuario.Role = usuarioActualizado.Role;

            await _usuarioRepo.ActualizarAsync(usuario);
            return Ok("Usuario actualizado con éxito!");
        }

        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            if (id <= 0)
                return BadRequest("ID inválido.");

            var usuario = await _usuarioRepo.ObtenerPorIdAsync(id);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            await _usuarioRepo.EliminarAsync(id);
            return Ok("Usuario eliminado con éxito!");
        }
    }
}
