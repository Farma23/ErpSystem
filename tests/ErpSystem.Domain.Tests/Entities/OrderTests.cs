using ErpSystem.Domain.Entities;
using ErpSystem.Domain.Enums;
using ErpSystem.Domain.Exceptions;
using ErpSystem.Domain.ValueObjects;
using Xunit;

namespace ErpSystem.Domain.Tests.Entities;

public class OrderTests
{
    [Fact]
    public void Create_ValidCustomerId_ReturnsOrderWithDraftStatus()
    {
        var customerId = Guid.NewGuid();
        var order = Order.Create(customerId, "USD");
        Assert.Equal(OrderStatus.Draft, order.Status);
        Assert.Equal(customerId, order.CustomerId);
        Assert.Empty(order.Items);
    }

    [Fact]
    public void Create_EmptyCustomerId_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => Order.Create(Guid.Empty));
    }

    [Fact]
    public void AddItem_ValidItem_AddsItemToOrder()
    {
        var order = Order.Create(Guid.NewGuid(), "USD");
        var price = new Money(100, "USD");
        order.AddItem("Product A", 2, price);
        Assert.Single(order.Items);
    }

    [Fact]
    public void AddItem_ZeroQuantity_ThrowsDomainException()
    {
        var order = Order.Create(Guid.NewGuid(), "USD");
        var price = new Money(100, "USD");
        Assert.Throws<DomainException>(() => order.AddItem("Product A", 0, price));
    }

    [Fact]
    public void AddItem_ConfirmedOrder_ThrowsDomainException()
    {
        var order = Order.Create(Guid.NewGuid(), "USD");
        order.AddItem("Product A", 1, new Money(100, "USD"));
        order.Confirm();
        Assert.Throws<DomainException>(() => order.AddItem("Product B", 1, new Money(50, "USD")));
    }

    [Fact]
    public void Confirm_DraftOrderWithItems_ChangesStatusToConfirmed()
    {
        var order = Order.Create(Guid.NewGuid(), "USD");
        order.AddItem("Product A", 1, new Money(100, "USD"));
        order.Confirm();
        Assert.Equal(OrderStatus.Confirmed, order.Status);
    }

    [Fact]
    public void Confirm_DraftOrderWithNoItems_ThrowsDomainException()
    {
        var order = Order.Create(Guid.NewGuid(), "USD");
        Assert.Throws<DomainException>(() => order.Confirm());
    }

    [Fact]
    public void Cancel_DraftOrder_ChangesStatusToCancelled()
    {
        var order = Order.Create(Guid.NewGuid(), "USD");
        order.Cancel();
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_CancelledOrder_ThrowsDomainException()
    {
        var order = Order.Create(Guid.NewGuid(), "USD");
        order.Cancel();
        Assert.Throws<DomainException>(() => order.Cancel());
    }
}
