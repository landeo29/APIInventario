using APIInventario.Infrastructure.Repositories.Implementations;
using APIInventario.Infrastructure.Repositories.Interfaces;
using APIInventario.Infrastructure.Repositories.Implementations;
using APIInventario.Core.Interfaces;
using APIInventario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using APIGestionInventarios.Infrastructure.Repositories.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();


builder.Services.AddControllers();

var app = builder.Build();
app.UseAuthorization();
app.MapControllers();
app.Run();
