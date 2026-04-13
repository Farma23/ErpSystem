using MediatR;

namespace ErpSystem.Application.Orders.Commands.AddItemToOrder;

public record AddItemToOrderCommand(
    Guid OrderId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    string Currency
) : IRequest;