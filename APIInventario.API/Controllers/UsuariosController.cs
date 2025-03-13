using APIInventario.Core.Interfaces;
using APIInventario.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

namespace APIINVENTARIO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepo;

        public UsuariosController(IUsuarioRepository usuarioRepo)
        {
            _usuarioRepo = usuarioRepo;
        }

        [HttpGet("listar")]
        [Authorize(Roles = "admin,empleado")]
        public async Task<IActionResult> ListarUsuarios()
        {
            var usuarios = await _usuarioRepo.ObtenerTodosAsync();
            if (usuarios == null || !usuarios.Any())
                return NotFound("No hay usuarios registrados.");
            return Ok(usuarios);
        }

        [HttpPost("crear")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CrearUsuario([FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos.");

            if (string.IsNullOrWhiteSpace(usuario.Username) || string.IsNullOrWhiteSpace(usuario.Password))
                return BadRequest("El nombre de usuario y la contraseña son obligatorios.");

            var existeUsuario = await _usuarioRepo.ObtenerPorNombreAsync(usuario.Username);
            if (existeUsuario != null)
                return Conflict("El nombre de usuario ya está en uso.");

            usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);

            await _usuarioRepo.AgregarAsync(usuario);
            return Ok("Usuario creado con éxito!");
        }

        [HttpGet("obtener/{id}")]
        [Authorize(Roles = "admin,empleado")]
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
            if (usuarioActualizado == null || id != usuarioActualizado.Id)
                return BadRequest("Los datos enviados son inválidos.");

            var usuarioExistente = await _usuarioRepo.ObtenerPorIdAsync(id);
            if (usuarioExistente == null)
                return NotFound("Usuario no encontrado.");

            usuarioExistente.Username = usuarioActualizado.Username;
            usuarioExistente.Role = usuarioActualizado.Role;

            if (!string.IsNullOrWhiteSpace(usuarioActualizado.Password))
            {
                usuarioExistente.Password = BCrypt.Net.BCrypt.HashPassword(usuarioActualizado.Password);
            }

            await _usuarioRepo.ActualizarAsync(usuarioExistente);
            return Ok(new { mensaje = "Usuario actualizado correctamente" });
        }




        [HttpDelete("eliminar/{id}")]
        [Authorize(Roles = "admin")]
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
