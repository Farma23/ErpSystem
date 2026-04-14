using ErpSystem.Application.Common.Exceptions;
using ErpSystem.Application.Orders.Queries.GetOrderById;
using ErpSystem.Domain.Entities;
using ErpSystem.Domain.Interfaces;
using MediatR;

namespace ErpSystem.Application.Orders.Queries.GetOrderById;

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IRepository<Order> _orders;

    public GetOrderByIdHandler(IRepository<Order> orders) => _orders = orders;

    public async Task<OrderDto> Handle(GetOrderByIdQuery query, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(query.OrderId, ct)
            ?? throw new NotFoundException(nameof(Order), query.OrderId);

        var total = order.Items.Sum(i => i.SubTotal.Amount);

        return new OrderDto(
            order.Id,
            order.OrderNumber,
            order.CustomerId,
            order.Status,
            total,
            order.Total.Currency,
            order.CreatedAt,
            order.Items.Select(i => new OrderItemDto(
                i.Id,
                i.ProductName,
                i.Quantity,
                i.UnitPrice.Amount,
                i.SubTotal.Amount
            )).ToList()
        );
    }
}
