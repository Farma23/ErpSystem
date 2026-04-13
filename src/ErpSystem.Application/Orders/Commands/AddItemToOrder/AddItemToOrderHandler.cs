using ErpSystem.Application.Common.Exceptions;
using ErpSystem.Domain.Entities;
using ErpSystem.Domain.Interfaces;
using ErpSystem.Domain.ValueObjects;
using MediatR;

namespace ErpSystem.Application.Orders.Commands.AddItemToOrder;

public class AddItemToOrderHandler : IRequestHandler<AddItemToOrderCommand>
{
    private readonly IRepository<Order> _orders;
    private readonly IUnitOfWork _uow;

    public AddItemToOrderHandler(IRepository<Order> orders, IUnitOfWork uow)
        => (_orders, _uow) = (orders, uow);

    public async Task Handle(AddItemToOrderCommand cmd, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(cmd.OrderId, ct)
            ?? throw new NotFoundException(nameof(Order), cmd.OrderId);

        var price = new Money(cmd.UnitPrice, cmd.Currency);
        order.AddItem(cmd.ProductName, cmd.Quantity, price);
        _orders.Update(order);
        await _uow.CommitAsync(ct);
    }
}