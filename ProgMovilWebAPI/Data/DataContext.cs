
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

        // Nuevas tablas de ventas
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Username único
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Username)
                .IsUnique();
            modelBuilder.Entity<Cliente>()
    .HasOne(c => c.Usuario)
    .WithOne(u => u.Cliente)
    .HasForeignKey<Cliente>(c => c.UsuarioId)
    .OnDelete(DeleteBehavior.Restrict);

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

            // IngresoProducto -> Usuario
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

            // SalidaProducto -> Usuario
            modelBuilder.Entity<SalidaProducto>()
                .HasOne(s => s.Usuario)
                .WithMany()
                .HasForeignKey(s => s.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Venta -> Usuario
            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Usuario)
                .WithMany()
                .HasForeignKey(v => v.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Venta -> Detalles
            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Venta)
                .WithMany(v => v.Detalles)
                .HasForeignKey(d => d.VentaId)
                .OnDelete(DeleteBehavior.Restrict);

            // DetalleVenta -> Producto
            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Importes monetarios
            modelBuilder.Entity<Venta>()
                .Property(v => v.Total)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DetalleVenta>()
                .Property(d => d.PrecioUnitario)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DetalleVenta>()
                .Property(d => d.Subtotal)
                .HasPrecision(18, 2);

            // Restricciones de integridad
            modelBuilder.Entity<DetalleVenta>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_DetalleVenta_Cantidad",
                    "[Cantidad] > 0"));

            modelBuilder.Entity<Venta>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Venta_Total",
                    "[Total] >= 0"));
        }
    }
}
