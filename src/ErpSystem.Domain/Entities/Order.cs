using ErpSystem.Domain.Common;
using ErpSystem.Domain.Enums;
using ErpSystem.Domain.Exceptions;
using ErpSystem.Domain.ValueObjects;

namespace ErpSystem.Domain.Entities;

public class Order : BaseEntity
{
    public string OrderNumber { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money Total { get; private set; } = Money.Zero("USD");

    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    public static Order Create(Guid customerId, string currency = "USD")
    {
        if (customerId == Guid.Empty)
            throw new DomainException("CustomerId is required.");

        return new Order
        {
            CustomerId = customerId,
            Status = OrderStatus.Draft,
            OrderNumber = GenerateOrderNumber(),
            Total = Money.Zero(currency)
        };
    }

    public void AddItem(string productName, int quantity, Money unitPrice)
    {
        if (Status != OrderStatus.Draft)
            throw new DomainException("Items can only be added to Draft orders.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        var item = new OrderItem(Id, productName, quantity, unitPrice);
        _items.Add(item);
        SetUpdatedAt();
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Draft)
            throw new DomainException("Only Draft orders can be confirmed.");

        if (!_items.Any())
            throw new DomainException("Cannot confirm an order with no items.");

        Status = OrderStatus.Confirmed;
        SetUpdatedAt();
    }

    public void Cancel()
    {
        if (Status is OrderStatus.Delivered or OrderStatus.Cancelled)
            throw new DomainException($"Cannot cancel an order in status {Status}.");

        Status = OrderStatus.Cancelled;
        SetUpdatedAt();
    }

    private static string GenerateOrderNumber()
        => $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
}
