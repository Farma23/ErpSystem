using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ErpSystem.Infrastructure.Identity;

/// <summary>
/// Servicio responsable de generar tokens JWT para autenticación.
/// Lee la configuración de JWT desde appsettings.json (sección "Jwt")
/// y construye tokens firmados con HMAC-SHA256.
/// Los tokens incluyen claims del usuario (id, email, roles) y tienen
/// un tiempo de expiración configurable.
/// </summary>
public class JwtTokenGenerator : ErpSystem.Application.Common.Interfaces.IJwtTokenGenerator
{
    /// <summary>
    /// Configuración de la aplicación inyectada para leer los valores de JWT
    /// como el secret, issuer, audience y tiempo de expiración.
    /// </summary>
    private readonly IConfiguration _config;

    /// <summary>
    /// Constructor que recibe la configuración mediante inyección de dependencias.
    /// </summary>
    /// <param name="config">Configuración de la aplicación.</param>
    public JwtTokenGenerator(IConfiguration config) => _config = config;

    /// <summary>
    /// Genera un token JWT firmado para el usuario especificado.
    /// El token incluye los claims estándar (sub, email, jti) más los roles del usuario.
    /// La firma usa HMAC-SHA256 con el secret definido en appsettings.json.
    /// </summary>
    /// <param name="userId">Identificador único del usuario autenticado.</param>
    /// <param name="email">Email del usuario, incluido como claim en el token.</param>
    /// <param name="roles">Lista de roles del usuario para control de acceso.</param>
    /// <returns>Token JWT firmado como string.</returns>
    public string GenerateToken(Guid userId, string email, IList<string> roles)
    {
        // Clave de firma derivada del secret en appsettings.json
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Claims estándar del token
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Agregar un claim por cada rol del usuario
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(
                double.Parse(_config["Jwt:ExpirationHours"] ?? "8")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}