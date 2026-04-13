using ErpSystem.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ErpSystem.Infrastructure.Services;

/// <summary>
/// Servicio que expone información del usuario autenticado en el request actual.
/// Lee los claims del token JWT desde el HttpContext, que ASP.NET Core
/// popula automáticamente al validar el token en el middleware de autenticación.
/// Implementa ICurrentUser para que la capa Application pueda acceder
/// al usuario sin depender de HttpContext directamente.
/// </summary>
public class CurrentUserService : ICurrentUser
{
    /// <summary>
    /// Accessor del contexto HTTP actual, inyectado por el contenedor de DI.
    /// Permite acceder al HttpContext fuera de los controllers,
    /// como en servicios y repositorios.
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Constructor que recibe el IHttpContextAccessor mediante inyección de dependencias.
    /// </summary>
    /// <param name="httpContextAccessor">Accessor del contexto HTTP actual.</param>
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    /// <summary>
    /// Identificador único del usuario autenticado.
    /// Lee el claim NameIdentifier del token JWT.
    /// Retorna Guid.Empty si el usuario no está autenticado.
    /// </summary>
    public Guid UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?
                .User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : Guid.Empty;
        }
    }

    /// <summary>
    /// Email del usuario autenticado.
    /// Lee el claim Email del token JWT.
    /// Retorna string vacío si el usuario no está autenticado.
    /// </summary>
    public string Email
        => _httpContextAccessor.HttpContext?
            .User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

    /// <summary>
    /// Indica si el usuario actual está autenticado.
    /// Verifica la identidad del usuario en el contexto HTTP actual.
    /// </summary>
    public bool IsAuthenticated
        => _httpContextAccessor.HttpContext?
            .User.Identity?.IsAuthenticated ?? false;
}