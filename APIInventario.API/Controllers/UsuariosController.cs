using APIInventario.Core.Interfaces;
using APIInventario.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace APIINVENTARIO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            return Ok(usuarios);
        }

        [HttpPost("crear")]
        public async Task<IActionResult> CrearUsuario([FromBody] Usuario usuario)
        {
            await _usuarioRepo.AgregarAsync(usuario);
            return Ok("Usuario creado con éxito!");
        }
    }
}
