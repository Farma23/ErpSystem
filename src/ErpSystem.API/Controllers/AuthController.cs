using ErpSystem.Application.Auth.Commands.Login;
using ErpSystem.Application.Auth.Commands.Register;
using Microsoft.AspNetCore.Mvc;

namespace ErpSystem.API.Controllers;

/// <summary>
/// Controller que expone los endpoints de autenticación del ERP.
/// Maneja registro de nuevos usuarios y login con generación de JWT.
/// Hereda de BaseApiController para acceder a MediatR sin inyección manual.
/// </summary>
public class AuthController : BaseApiController
{
    /// <summary>
    /// Registra un nuevo usuario en el sistema.
    /// POST /api/auth/register
    /// Retorna el token JWT generado tras el registro exitoso.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand cmd, CancellationToken ct)
    {
        var result = await Mediator.Send(cmd, ct);
        return Ok(result);
    }

    /// <summary>
    /// Autentica un usuario existente y retorna un token JWT.
    /// POST /api/auth/login
    /// El token debe incluirse en el header Authorization: Bearer {token}
    /// en todas las peticiones protegidas.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand cmd, CancellationToken ct)
    {
        var result = await Mediator.Send(cmd, ct);
        return Ok(result);
    }
}
