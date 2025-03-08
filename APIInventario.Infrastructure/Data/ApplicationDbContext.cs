using System.Collections.Generic;
using System.Reflection.Emit;
using APIINVENTARIO.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace APIINVENTARIO.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; } = null!;
        //public DbSet<Producto> Productos { get; set; } = null!;
        //public DbSet<Categoria> Categorias { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            //modelBuilder.Entity<Producto>().ToTable("Productos");
            //modelBuilder.Entity<Categoria>().ToTable("Categorias");
        }
    }
}
