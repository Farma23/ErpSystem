using MediatR;

namespace ErpSystem.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(
    Guid CustomerId,
    string Currency = "USD"
) : IRequest<Guid>;