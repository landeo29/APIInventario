using APIInventario.Core.Models;
using APIInventario.Infrastructure.Data;
using APIInventario.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIInventario.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;

        public AuthController(ApplicationDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Usuario usuario)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Username == usuario.Username))
                return BadRequest("El usuario ya existe.");

            usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok("Usuario registrado con éxito!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest request)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario.Password))
                return Unauthorized("Credenciales inválidas.");

            var token = _tokenService.GenerarToken(usuario);
            return Ok(new AuthResponse { Token = token, Role = usuario.Role });
        }
    }
}
