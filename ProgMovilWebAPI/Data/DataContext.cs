
using Microsoft.EntityFrameworkCore;
using ProgMovilWebAPI.Models;

namespace ProgMovilWebAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Imagen> Imagenes { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<IngresoProducto> IngresosProductos { get; set; }
        public DbSet<SalidaProducto> SalidasProductos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Username único
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Precio del producto
            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasPrecision(18, 2);

            // Un producto puede tener solamente una imagen
            modelBuilder.Entity<Imagen>()
                .HasIndex(i => i.ProductoId)
                .IsUnique();

            // Imagen -> Producto
            modelBuilder.Entity<Imagen>()
                .HasOne(i => i.Producto)
                .WithMany(p => p.Imagenes)
                .HasForeignKey(i => i.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            // IngresoProducto -> Producto
            modelBuilder.Entity<IngresoProducto>()
                .HasOne(i => i.Producto)
                .WithMany()
                .HasForeignKey(i => i.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            // IngresoProducto -> Proveedor
            modelBuilder.Entity<IngresoProducto>()
                .HasOne(i => i.Proveedor)
                .WithMany()
                .HasForeignKey(i => i.ProveedorId)
                .OnDelete(DeleteBehavior.Restrict);

            // NUEVO: IngresoProducto -> Usuario
            modelBuilder.Entity<IngresoProducto>()
                .HasOne(i => i.Usuario)
                .WithMany()
                .HasForeignKey(i => i.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // SalidaProducto -> Producto
            modelBuilder.Entity<SalidaProducto>()
                .HasOne(s => s.Producto)
                .WithMany()
                .HasForeignKey(s => s.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            // SalidaProducto -> Cliente
            modelBuilder.Entity<SalidaProducto>()
                .HasOne(s => s.Cliente)
                .WithMany()
                .HasForeignKey(s => s.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // NUEVO: SalidaProducto -> Usuario
            modelBuilder.Entity<SalidaProducto>()
                .HasOne(s => s.Usuario)
                .WithMany()
                .HasForeignKey(s => s.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
