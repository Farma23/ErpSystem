using ErpSystem.Domain.Interfaces;

namespace ErpSystem.Infrastructure.Persistence;

/// <summary>
/// Implementación del patrón Unit of Work usando Entity Framework Core.
/// Coordina la escritura de cambios hacia la base de datos en una sola transacción.
/// Todos los repositorios comparten el mismo DbContext dentro de un request,
/// por lo que CommitAsync persiste todos los cambios pendientes de todos los repositorios
/// en una sola operación atómica.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    /// <summary>
    /// Contexto de base de datos compartido con todos los repositorios del request.
    /// Al ser Scoped, la misma instancia de AppDbContext se inyecta tanto aquí
    /// como en todos los repositorios dentro del mismo request HTTP.
    /// </summary>
    private readonly AppDbContext _db;

    /// <summary>
    /// Constructor que recibe el DbContext mediante inyección de dependencias.
    /// </summary>
    /// <param name="db">Contexto de base de datos de la aplicación.</param>
    public UnitOfWork(AppDbContext db) => _db = db;

    /// <summary>
    /// Persiste todos los cambios pendientes en la base de datos.
    /// Llama a SaveChangesAsync de EF Core, que ejecuta todas las operaciones
    /// INSERT, UPDATE y DELETE acumuladas en el contexto en una sola transacción.
    /// </summary>
    /// <param name="ct">Token de cancelación para operaciones asíncronas.</param>
    /// <returns>Número de filas afectadas en la base de datos.</returns>
    public async Task<int> CommitAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);
}