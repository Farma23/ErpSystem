using ErpSystem.Application.Orders.Commands.AddItemToOrder;
using ErpSystem.Application.Orders.Commands.CancelOrder;
using ErpSystem.Application.Orders.Commands.ConfirmOrder;
using ErpSystem.Application.Orders.Commands.CreateOrder;
using ErpSystem.Application.Orders.Queries.GetAllOrders;
using ErpSystem.Application.Orders.Queries.GetOrderById;
using Microsoft.AspNetCore.Mvc;

namespace ErpSystem.API.Controllers;

/// <summary>
/// Controller que expone los endpoints REST para gestión de órdenes.
/// Hereda de BaseApiController para acceder a MediatR sin inyección manual.
/// Sigue el patrón CQRS: cada acción despacha un Command o Query a MediatR.
/// </summary>
public class OrdersController : BaseApiController
{
    /// <summary>
    /// Obtiene todas las órdenes del sistema.
    /// GET /api/orders
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await Mediator.Send(new GetAllOrdersQuery(), ct);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene una orden específica por su ID.
    /// GET /api/orders/{id}
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new GetOrderByIdQuery(id), ct);
        return Ok(result);
    }

    /// <summary>
    /// Crea una nueva orden en estado Draft.
    /// POST /api/orders
    /// Retorna 201 Created con el ID de la orden creada.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderCommand cmd, CancellationToken ct)
    {
        var id = await Mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Agrega un ítem a una orden existente en estado Draft.
    /// POST /api/orders/{id}/items
    /// </summary>
    [HttpPost("{id:guid}/items")]
    public async Task<IActionResult> AddItem(Guid id, [FromBody] AddItemToOrderCommand cmd, CancellationToken ct)
    {
        await Mediator.Send(cmd with { OrderId = id }, ct);
        return NoContent();
    }

    /// <summary>
    /// Confirma una orden, cambiando su estado de Draft a Confirmed.
    /// POST /api/orders/{id}/confirm
    /// </summary>
    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken ct)
    {
        await Mediator.Send(new ConfirmOrderCommand(id), ct);
        return NoContent();
    }

    /// <summary>
    /// Cancela una orden que no haya sido entregada aún.
    /// POST /api/orders/{id}/cancel
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        await Mediator.Send(new CancelOrderCommand(id), ct);
        return NoContent();
    }
}
