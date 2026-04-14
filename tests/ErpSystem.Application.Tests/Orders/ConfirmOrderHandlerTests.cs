using ErpSystem.Application.Common.Exceptions;
using ErpSystem.Application.Orders.Commands.ConfirmOrder;
using ErpSystem.Domain.Entities;
using ErpSystem.Domain.Interfaces;
using ErpSystem.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace ErpSystem.Application.Tests.Orders;

public class ConfirmOrderHandlerTests
{
    private readonly Mock<IRepository<Order>> _ordersMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();

    [Fact]
    public async Task Handle_ExistingOrderWithItems_ConfirmsOrder()
    {
        var order = Order.Create(Guid.NewGuid(), "USD");
        order.AddItem("Product", 1, new Money(100, "USD"));

        _ordersMock.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var handler = new ConfirmOrderHandler(_ordersMock.Object, _uowMock.Object);
        var cmd = new ConfirmOrderCommand(order.Id);

        await handler.Handle(cmd, CancellationToken.None);

        order.Status.Should().Be(Domain.Enums.OrderStatus.Confirmed);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingOrder_ThrowsNotFoundException()
    {
        _ordersMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var handler = new ConfirmOrderHandler(_ordersMock.Object, _uowMock.Object);
        var cmd = new ConfirmOrderCommand(Guid.NewGuid());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(cmd, CancellationToken.None));
    }
}
