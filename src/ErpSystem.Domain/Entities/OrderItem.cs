using ErpSystem.Domain.Common;
using ErpSystem.Domain.Exceptions;
using ErpSystem.Domain.ValueObjects;

namespace ErpSystem.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = default!;
    public Money SubTotal => new(UnitPrice.Amount * Quantity, UnitPrice.Currency);

    private OrderItem() { } // EF Core

    internal OrderItem(Guid orderId, string productName, int quantity, Money unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new DomainException("ProductName is required.");

        OrderId = orderId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}