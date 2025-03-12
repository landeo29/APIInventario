using Moq;
using APIInventario.Core.Models;
using APIInventario.Core.Interfaces;
using APIINVENTARIO.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace APIINVENTARIO.Tests.Controllers
{
    public class UsuariosControllerTests
    {
        private readonly UsuariosController _controller;
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepo;

        public UsuariosControllerTests()
        {
            _mockUsuarioRepo = new Mock<IUsuarioRepository>();
            _controller = new UsuariosController(_mockUsuarioRepo.Object);
        }

        #region Test para ListarUsuarios

        [Fact]
        public async Task ListarUsuarios_ReturnsOk_WhenUsersExist()
        {
            var usuarios = new List<Usuario> { new Usuario { Id = 1, Username = "user1" }, new Usuario { Id = 2, Username = "user2" } };
            _mockUsuarioRepo.Setup(repo => repo.ObtenerTodosAsync()).ReturnsAsync(usuarios);

            var result = await _controller.ListarUsuarios();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnUsuarios = Assert.IsAssignableFrom<IEnumerable<Usuario>>(okResult.Value);
            Assert.Equal(2, returnUsuarios.Count());
        }

        [Fact]
        public async Task ListarUsuarios_ReturnsNotFound_WhenNoUsersExist()
        {
            _mockUsuarioRepo.Setup(repo => repo.ObtenerTodosAsync()).ReturnsAsync(new List<Usuario>());

            var result = await _controller.ListarUsuarios();

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No hay usuarios registrados.", notFoundResult.Value);
        }

        #endregion

        #region Test para CrearUsuario

        [Fact]
        public async Task CrearUsuario_ReturnsBadRequest_WhenModelIsInvalid()
        {
            var usuario = new Usuario { Username = "", Password = "" };
            _controller.ModelState.AddModelError("Username", "El nombre de usuario es obligatorio");

            var result = await _controller.CrearUsuario(usuario);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Datos inválidos.", badRequestResult.Value);
        }

        [Fact]
        public async Task CrearUsuario_ReturnsBadRequest_WhenUsernameOrPasswordIsEmpty()
        {
            var usuario = new Usuario { Username = "", Password = "password" };

            var result = await _controller.CrearUsuario(usuario);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("El nombre de usuario y la contraseña son obligatorios.", badRequestResult.Value);
        }

        [Fact]
        public async Task CrearUsuario_ReturnsConflict_WhenUsernameAlreadyExists()
        {
            var usuario = new Usuario { Username = "existingUser", Password = "password" };
            _mockUsuarioRepo.Setup(repo => repo.ObtenerPorNombreAsync(usuario.Username)).ReturnsAsync(new Usuario());

            var result = await _controller.CrearUsuario(usuario);

            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("El nombre de usuario ya está en uso.", conflictResult.Value);
        }

        [Fact]
        public async Task CrearUsuario_ReturnsOk_WhenUserIsCreatedSuccessfully()
        {
            // Arrange
            var usuario = new Usuario { Username = "newUser", Password = "password", Role = "user" };
            _mockUsuarioRepo.Setup(repo => repo.ObtenerPorNombreAsync(usuario.Username)).ReturnsAsync((Usuario)null); 

            var result = await _controller.CrearUsuario(usuario);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Usuario creado con éxito!", okResult.Value);
        }

        #endregion

        #region Test para ObtenerUsuario

        [Fact]
        public async Task ObtenerUsuario_ReturnsBadRequest_WhenIdIsInvalid()
        {
            var result = await _controller.ObtenerUsuario(-1);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID inválido.", badRequestResult.Value);
        }

        [Fact]
        public async Task ObtenerUsuario_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var usuarioId = 1;
            _mockUsuarioRepo.Setup(repo => repo.ObtenerPorIdAsync(usuarioId)).ReturnsAsync((Usuario)null);

            var result = await _controller.ObtenerUsuario(usuarioId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Usuario no encontrado.", notFoundResult.Value);
        }

        [Fact]
        public async Task ObtenerUsuario_ReturnsOk_WhenUserExists()
        {
            var usuario = new Usuario { Id = 1, Username = "user1" };
            _mockUsuarioRepo.Setup(repo => repo.ObtenerPorIdAsync(usuario.Id)).ReturnsAsync(usuario);

            var result = await _controller.ObtenerUsuario(usuario.Id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnUsuario = Assert.IsType<Usuario>(okResult.Value);
            Assert.Equal("user1", returnUsuario.Username);
        }

        #endregion

        #region Test para ActualizarUsuario

        [Fact]
        public async Task ActualizarUsuario_ReturnsBadRequest_WhenIdIsInvalid()
        {
            var result = await _controller.ActualizarUsuario(-1, new Usuario());

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID inválido.", badRequestResult.Value);
        }

        [Fact]
        public async Task ActualizarUsuario_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var usuarioId = 1;
            _mockUsuarioRepo.Setup(repo => repo.ObtenerPorIdAsync(usuarioId)).ReturnsAsync((Usuario)null);

            var result = await _controller.ActualizarUsuario(usuarioId, new Usuario());

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Usuario no encontrado.", notFoundResult.Value);
        }

        [Fact]
        public async Task ActualizarUsuario_ReturnsOk_WhenUserIsUpdatedSuccessfully()
        {
            var usuarioId = 1;
            var usuarioExistente = new Usuario { Id = usuarioId, Username = "oldUser" };
            var usuarioActualizado = new Usuario { Username = "updatedUser", Password = "newPassword", Role = "admin" };
            _mockUsuarioRepo.Setup(repo => repo.ObtenerPorIdAsync(usuarioId)).ReturnsAsync(usuarioExistente);

            var result = await _controller.ActualizarUsuario(usuarioId, usuarioActualizado);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Usuario actualizado con éxito!", okResult.Value);
        }

        #endregion

        #region Test para EliminarUsuario

        [Fact]
        public async Task EliminarUsuario_ReturnsBadRequest_WhenIdIsInvalid()
        {
            var result = await _controller.EliminarUsuario(-1);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID inválido.", badRequestResult.Value);
        }

        [Fact]
        public async Task EliminarUsuario_ReturnsNotFound_WhenUserDoesNotExist()
        {
            
            var usuarioId = 1;
            _mockUsuarioRepo.Setup(repo => repo.ObtenerPorIdAsync(usuarioId)).ReturnsAsync((Usuario)null);

            var result = await _controller.EliminarUsuario(usuarioId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Usuario no encontrado.", notFoundResult.Value);
        }

        [Fact]
        public async Task EliminarUsuario_ReturnsOk_WhenUserIsDeletedSuccessfully()
        {
            var usuarioId = 1;
            var usuarioExistente = new Usuario { Id = usuarioId, Username = "userToDelete" };
            _mockUsuarioRepo.Setup(repo => repo.ObtenerPorIdAsync(usuarioId)).ReturnsAsync(usuarioExistente);


            var result = await _controller.EliminarUsuario(usuarioId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Usuario eliminado con éxito!", okResult.Value);
        }

        #endregion
    }
}
