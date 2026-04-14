using ErpSystem.Application.Common.Exceptions;
using ErpSystem.Domain.Entities;
using ErpSystem.Domain.Interfaces;
using ErpSystem.Domain.ValueObjects;
using MediatR;

namespace ErpSystem.Application.Orders.Commands.AddItemToOrder;

public class AddItemToOrderHandler : IRequestHandler<AddItemToOrderCommand>
{
    private readonly IRepository<Order> _orders;
    private readonly IRepository<OrderItem> _items;
    private readonly IUnitOfWork _uow;

    public AddItemToOrderHandler(IRepository<Order> orders, IRepository<OrderItem> items, IUnitOfWork uow)
        => (_orders, _items, _uow) = (orders, items, uow);

    public async Task Handle(AddItemToOrderCommand cmd, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(cmd.OrderId, ct)
            ?? throw new NotFoundException(nameof(Order), cmd.OrderId);

        if (order.Status != Domain.Enums.OrderStatus.Draft)
            throw new Domain.Exceptions.DomainException("Items can only be added to Draft orders.");

        var price = new Money(cmd.UnitPrice, cmd.Currency);
        var item = new OrderItem(cmd.OrderId, cmd.ProductName, cmd.Quantity, price);
        await _items.AddAsync(item, ct);
        await _uow.CommitAsync(ct);
    }
}
