using ErpSystem.Application.Orders.Commands.CreateOrder;
using ErpSystem.Domain.Entities;
using ErpSystem.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ErpSystem.Application.Tests.Orders;

public class CreateOrderHandlerTests
{
    private readonly Mock<IRepository<Order>> _ordersMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();

    [Fact]
    public async Task Handle_ValidCommand_ReturnsOrderId()
    {
        var handler = new CreateOrderHandler(_ordersMock.Object, _uowMock.Object);
        var cmd = new CreateOrderCommand(Guid.NewGuid(), "USD");

        var result = await handler.Handle(cmd, CancellationToken.None);

        result.Should().NotBeEmpty();
        _ordersMock.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyCustomerId_ThrowsDomainException()
    {
        var handler = new CreateOrderHandler(_ordersMock.Object, _uowMock.Object);
        var cmd = new CreateOrderCommand(Guid.Empty, "USD");

        await Assert.ThrowsAsync<Domain.Exceptions.DomainException>(
            () => handler.Handle(cmd, CancellationToken.None));
    }
}
