using ErpSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ErpSystem.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de mapeo de la entidad OrderItem hacia la base de datos.
/// Define cómo EF Core traduce cada ítem de una orden a una fila en la tabla OrderItems,
/// incluyendo el mapeo del Value Object Money (UnitPrice) como columnas propias.
/// </summary>
public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    /// <summary>
    /// Define la configuración completa de la tabla OrderItems.
    /// Este método es llamado automáticamente por EF Core durante OnModelCreating.
    /// </summary>
    /// <param name="builder">Constructor de configuración para la entidad OrderItem.</param>
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        // Clave primaria
        builder.HasKey(i => i.Id);

        // Nombre del producto: obligatorio con longitud máxima
        builder.Property(i => i.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        // Cantidad: obligatoria, debe ser mayor que cero (validado en el dominio)
        builder.Property(i => i.Quantity)
            .IsRequired();

        // Precio unitario: es un Value Object (Money) mapeado como Owned Entity.
        // Se guarda en columnas de la misma tabla OrderItems.
        // UnitPriceAmount: valor decimal con precisión monetaria
        // UnitPriceCurrency: código ISO de 3 letras
        builder.OwnsOne(i => i.UnitPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("UnitPriceAmount")
                .HasPrecision(18, 2);

            money.Property(m => m.Currency)
                .HasColumnName("UnitPriceCurrency")
                .HasMaxLength(3);
        });

        // SubTotal es una propiedad calculada en memoria (Quantity * UnitPrice).
        // No se persiste en la base de datos, se calcula al leer la entidad.
        builder.Ignore(i => i.SubTotal);

        // Los DomainEvents son solo para uso en memoria durante el request.
        builder.Ignore(i => i.DomainEvents);
    }
}