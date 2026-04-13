using ErpSystem.Domain.Enums;

namespace ErpSystem.Application.Orders.Queries.GetOrderById;

public record OrderDto(
    Guid Id,
    string OrderNumber,
    Guid CustomerId,
    OrderStatus Status,
    decimal Total,
    string Currency,
    DateTime CreatedAt,
    List<OrderItemDto> Items
);

public record OrderItemDto(
    Guid Id,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal SubTotal
);