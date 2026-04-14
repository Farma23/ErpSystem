using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ErpSystem.API.Controllers;

/// <summary>
/// Controller base del que heredan todos los controllers del ERP.
/// Centraliza la inyección de IMediator para que los controllers hijos
/// no necesiten declarar su propio constructor solo para obtener el mediator.
/// Todos los controllers heredan de este en lugar de ControllerBase directamente.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _mediator;

    /// <summary>
    /// Instancia de MediatR obtenida desde el HttpContext.
    /// Se resuelve de forma lazy (solo cuando se necesita por primera vez).
    /// Usar ISender en lugar de IMediator expone solo el método Send,
    /// que es todo lo que los controllers necesitan.
    /// </summary>
    protected ISender Mediator
        => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
