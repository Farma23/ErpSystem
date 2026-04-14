using ErpSystem.Application.Orders.Queries.GetOrderById;
using ErpSystem.Domain.Entities;
using ErpSystem.Domain.Interfaces;
using MediatR;

namespace ErpSystem.Application.Orders.Queries.GetAllOrders;

public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersQuery, List<OrderDto>>
{
    private readonly IRepository<Order> _orders;

    public GetAllOrdersHandler(IRepository<Order> orders) => _orders = orders;

    public async Task<List<OrderDto>> Handle(GetAllOrdersQuery query, CancellationToken ct)
    {
        var orders = await _orders.GetAllAsync(ct);

        return orders.Select(order => new OrderDto(
            order.Id,
            order.OrderNumber,
            order.CustomerId,
            order.Status,
            order.Items.Sum(i => i.SubTotal.Amount),
            order.Total.Currency,
            order.CreatedAt,
            order.Items.Select(i => new OrderItemDto(
                i.Id,
                i.ProductName,
                i.Quantity,
                i.UnitPrice.Amount,
                i.SubTotal.Amount
            )).ToList()
        )).ToList();
    }
}
