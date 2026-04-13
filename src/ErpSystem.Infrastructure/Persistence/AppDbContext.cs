using ErpSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.Infrastructure.Persistence;

/// <summary>
/// Contexto principal de Entity Framework Core para el ERP.
/// Actúa como la unidad de trabajo y el punto de acceso a la base de datos.
/// Todas las entidades del dominio se registran aquí como DbSets.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Constructor que recibe las opciones de configuración del contexto.
    /// Las opciones se configuran en Infrastructure/DependencyInjection.cs
    /// donde se define el proveedor de base de datos (SQL Server, PostgreSQL, etc).
    /// </summary>
    /// <param name="options">Opciones de configuración del DbContext.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>
    /// Tabla de órdenes en la base de datos.
    /// Cada Order representa una orden de compra o venta en el ERP.
    /// </summary>
    public DbSet<Order> Orders => Set<Order>();

    /// <summary>
    /// Tabla de ítems de órdenes.
    /// Cada OrderItem representa un producto dentro de una orden.
    /// </summary>
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    /// <summary>
    /// Aplica todas las configuraciones de entidades definidas en el ensamblado.
    /// Busca automáticamente todas las clases que implementan IEntityTypeConfiguration
    /// dentro del proyecto Infrastructure (OrderConfiguration, OrderItemConfiguration, etc).
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo de EF Core.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}