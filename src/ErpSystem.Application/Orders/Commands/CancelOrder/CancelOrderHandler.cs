using ErpSystem.Application.Common.Exceptions;
using ErpSystem.Domain.Entities;
using ErpSystem.Domain.Interfaces;
using MediatR;

namespace ErpSystem.Application.Orders.Commands.CancelOrder;

public class CancelOrderHandler : IRequestHandler<CancelOrderCommand>
{
    private readonly IRepository<Order> _orders;
    private readonly IUnitOfWork _uow;

    public CancelOrderHandler(IRepository<Order> orders, IUnitOfWork uow)
        => (_orders, _uow) = (orders, uow);

    public async Task Handle(CancelOrderCommand cmd, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(cmd.OrderId, ct)
            ?? throw new NotFoundException(nameof(Order), cmd.OrderId);

        order.Cancel();
        _orders.Update(order);
        await _uow.CommitAsync(ct);
    }
}
