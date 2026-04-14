using ErpSystem.Application.Common.Interfaces;
using ErpSystem.Domain.Entities;
using ErpSystem.Domain.Interfaces;
using ErpSystem.Infrastructure.Identity;
using ErpSystem.Infrastructure.Persistence;
using ErpSystem.Infrastructure.Persistence.Repositories;
using ErpSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ErpSystem.Infrastructure;

/// <summary>
/// Clase estática de extensión para registrar todos los servicios
/// de la capa Infrastructure en el contenedor de inyección de dependencias.
/// Se llama una sola vez desde Program.cs al iniciar la aplicación.
/// Registra: DbContext, repositorios, Unit of Work, JWT, servicios de soporte.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra todos los servicios de Infrastructure en el contenedor de DI.
    /// Configura EF Core con SQL Server, autenticación JWT, repositorios
    /// y servicios de soporte como CurrentUserService y DateTimeProvider.
    /// </summary>
    /// <param name="services">Colección de servicios de ASP.NET Core.</param>
    /// <param name="configuration">Configuración de la aplicación (appsettings.json).</param>
    /// <returns>La misma colección de servicios para encadenamiento fluido.</returns>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Registrar DbContext con SQL Server.
        // Scoped: una instancia por request HTTP.
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        // Repositorios: Scoped para compartir el mismo DbContext por request
        services.AddScoped<IRepository<Order>, OrderRepository>();
        services.AddScoped<IRepository<OrderItem>, OrderItemRepository>();

        // Unit of Work: Scoped para coordinar todos los repositorios del request
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Servicios de soporte
        services.AddScoped<ICurrentUser, CurrentUserService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<JwtTokenGenerator>();

        // IHttpContextAccessor necesario para CurrentUserService
        services.AddHttpContextAccessor();

        // Configuración de autenticación JWT
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!))
                };
            });

        return services;
    }
}