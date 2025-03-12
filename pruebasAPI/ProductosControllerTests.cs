using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIInventario.API.Controllers;
using APIInventario.Core.Models;
using APIInventario.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Xunit;

public class ProductosControllerTests
{
    private readonly Mock<IProductoRepository> _mockProductoRepo;
    private readonly ProductosController _controller;

    public ProductosControllerTests()
    {
        _mockProductoRepo = new Mock<IProductoRepository>();
        _controller = new ProductosController(_mockProductoRepo.Object);
    }

    #region Test ListarProductos

    [Fact]
    public async Task ListarProductos_ReturnsOk_WhenProductsExist()
    {
        // Arrange
        var productos = new List<Producto>
    {
        new Producto { Id = 1, Nombre = "Producto 1", Descripcion = "Descripción 1", Precio = 100, Cantidad = 10, CategoriaId = 1 },
        new Producto { Id = 2, Nombre = "Producto 2", Descripcion = "Descripción 2", Precio = 200, Cantidad = 5, CategoriaId = 2 }
    };

        _mockProductoRepo.Setup(repo => repo.ObtenerTodosAsync()).ReturnsAsync(productos);

        // Act
        var result = await _controller.ListarProductos();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<object>>(okResult.Value);  

        Assert.Equal(2, returnValue.Count);

        foreach (var item in returnValue)
        {
            var producto = Assert.IsType<IDictionary<string, object>>(item);  

            Assert.Contains("Id", producto.Keys);
            Assert.Contains("Nombre", producto.Keys);
            Assert.Contains("Descripcion", producto.Keys);
            Assert.Contains("Precio", producto.Keys);
            Assert.Contains("Cantidad", producto.Keys);
            Assert.Contains("Categoria", producto.Keys);
        }
    }


    [Fact]
    public async Task ListarProductos_ReturnsNotFound_WhenNoProductsExist()
    {
        // Arrange
        _mockProductoRepo.Setup(repo => repo.ObtenerTodosAsync()).ReturnsAsync(new List<Producto>());

        // Act
        var result = await _controller.ListarProductos();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("¡No hay productos disponibles!", notFoundResult.Value);
    }

    #endregion

    #region Test CrearProducto

    [Fact]
    public async Task CrearProducto_ReturnsOk_WhenProductIsValid()
    {
      
        var producto = new Producto
        {
            Nombre = "Nuevo Producto",
            Descripcion = "Descripción",
            Precio = 50,
            Cantidad = 10,
            CategoriaId = 1
        };

        _mockProductoRepo.Setup(repo => repo.AgregarAsync(It.IsAny<Producto>())).Returns(Task.CompletedTask);

      
        var result = await _controller.CrearProducto(producto);

     
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("¡Producto creado con éxito!", okResult.Value);
    }

    [Fact]
    public async Task CrearProducto_ReturnsBadRequest_WhenProductIsInvalid()
    {
       
        _controller.ModelState.AddModelError("Nombre", "El nombre es obligatorio.");
        var producto = new Producto();

 
        var result = await _controller.CrearProducto(producto);


        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Datos inválidos.", badRequestResult.Value);
    }

    #endregion

    #region Test ObtenerProducto

    [Fact]
    public async Task ObtenerProducto_ReturnsOk_WhenProductExists()
    {
     
        var producto = new Producto
        {
            Id = 1,
            Nombre = "Producto 1",
            Descripcion = "Descripción 1",
            Precio = 100,
            Cantidad = 10,
            CategoriaId = 1
        };

        _mockProductoRepo.Setup(repo => repo.ObtenerPorIdAsync(1)).ReturnsAsync(producto);

    
        var result = await _controller.ObtenerProducto(1);

     
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Producto>(okResult.Value);
        Assert.Equal("Producto 1", returnValue.Nombre);
    }

    [Fact]
    public async Task ObtenerProducto_ReturnsNotFound_WhenProductDoesNotExist()
    {
 
        _mockProductoRepo.Setup(repo => repo.ObtenerPorIdAsync(1)).ReturnsAsync((Producto)null);

     
        var result = await _controller.ObtenerProducto(1);

  
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("¡Producto no encontrado!", notFoundResult.Value);
    }

    #endregion

    #region Test ActualizarProducto

    [Fact]
    public async Task ActualizarProducto_ReturnsOk_WhenProductIsUpdated()
    {
 
        var productoExistente = new Producto
        {
            Id = 1,
            Nombre = "Producto 1",
            Descripcion = "Descripción 1",
            Precio = 100,
            Cantidad = 10,
            CategoriaId = 1
        };

        var productoActualizado = new Producto
        {
            Nombre = "Producto Actualizado",
            Descripcion = "Descripción Actualizada",
            Precio = 150,
            Cantidad = 5,
            CategoriaId = 1
        };

        _mockProductoRepo.Setup(repo => repo.ObtenerPorIdAsync(1)).ReturnsAsync(productoExistente);
        _mockProductoRepo.Setup(repo => repo.ActualizarAsync(It.IsAny<Producto>())).Returns(Task.CompletedTask);

 
        var result = await _controller.ActualizarProducto(1, productoActualizado);

  
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("¡Producto actualizado con éxito!", okResult.Value);
    }

    [Fact]
    public async Task ActualizarProducto_ReturnsNotFound_WhenProductDoesNotExist()
    {
    
        var productoActualizado = new Producto
        {
            Nombre = "Producto Actualizado",
            Descripcion = "Descripción Actualizada",
            Precio = 150,
            Cantidad = 5,
            CategoriaId = 1
        };

        _mockProductoRepo.Setup(repo => repo.ObtenerPorIdAsync(1)).ReturnsAsync((Producto)null);


        var result = await _controller.ActualizarProducto(1, productoActualizado);

   
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("¡Producto no encontrado!", notFoundResult.Value);
    }

    #endregion

    #region Test EliminarProducto

    [Fact]
    public async Task EliminarProducto_ReturnsOk_WhenProductIsDeleted()
    {

        var producto = new Producto
        {
            Id = 1,
            Nombre = "Producto 1",
            Descripcion = "Descripción 1",
            Precio = 100,
            Cantidad = 10,
            CategoriaId = 1
        };

        _mockProductoRepo.Setup(repo => repo.ObtenerPorIdAsync(1)).ReturnsAsync(producto);
        _mockProductoRepo.Setup(repo => repo.EliminarAsync(1)).Returns(Task.CompletedTask);


        var result = await _controller.EliminarProducto(1);


        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("¡Producto eliminado con éxito!", okResult.Value);
    }

    [Fact]
    public async Task EliminarProducto_ReturnsNotFound_WhenProductDoesNotExist()
    {

        _mockProductoRepo.Setup(repo => repo.ObtenerPorIdAsync(1)).ReturnsAsync((Producto)null);


        var result = await _controller.EliminarProducto(1);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("¡Producto no encontrado!", notFoundResult.Value);
    }

    #endregion
}
