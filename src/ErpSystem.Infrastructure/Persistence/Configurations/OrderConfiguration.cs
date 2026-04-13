using ErpSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ErpSystem.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de mapeo de la entidad Order hacia la base de datos.
/// Define cómo EF Core debe traducir la entidad Order a una tabla SQL,
/// incluyendo columnas, restricciones, relaciones y conversiones de tipos.
/// Implementa el patrón Fluent API de EF Core.
/// </summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    /// <summary>
    /// Define la configuración completa de la tabla Orders.
    /// Este método es llamado automáticamente por EF Core durante OnModelCreating.
    /// </summary>
    /// <param name="builder">Constructor de configuración para la entidad Order.</param>
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Clave primaria
        builder.HasKey(o => o.Id);

        // Número de orden: obligatorio y con longitud máxima definida
        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        // Estado de la orden: se guarda como string en lugar de int
        // para mayor legibilidad en la base de datos (ej: "Confirmed" en lugar de 1)
        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Total: es un Value Object (Money) que se mapea como Owned Entity.
        // EF Core lo guarda en columnas de la misma tabla (no en tabla separada).
        // TotalAmount: valor decimal con precisión monetaria
        // TotalCurrency: código ISO de 3 letras (USD, EUR, etc)
        builder.OwnsOne(o => o.Total, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalAmount")
                .HasPrecision(18, 2);

            money.Property(m => m.Currency)
                .HasColumnName("TotalCurrency")
                .HasMaxLength(3);
        });

        // Relación uno a muchos con OrderItems.
        // Al eliminar una Order, sus Items se eliminan en cascada.
        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Los DomainEvents son solo para uso en memoria durante el request.
        // No se persisten en la base de datos.
        builder.Ignore(o => o.DomainEvents);
    }
}