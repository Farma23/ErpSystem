using ErpSystem.Application.Orders.Queries.GetOrderById;
using MediatR;

namespace ErpSystem.Application.Orders.Queries.GetAllOrders;

public record GetAllOrdersQuery : IRequest<List<OrderDto>>;