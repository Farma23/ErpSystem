using ErpSystem.Domain.Entities;
using ErpSystem.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación concreta del repositorio de órdenes usando Entity Framework Core.
/// Esta clase vive en Infrastructure y implementa la interfaz IRepository definida en Domain,
/// siguiendo el principio de inversión de dependencias de Clean Architecture.
/// La capa Application solo conoce la interfaz, nunca esta implementación directamente.
/// </summary>
public class OrderRepository : IRepository<Order>
{
    /// <summary>
    /// Contexto de base de datos inyectado por el contenedor de DI.
    /// El ciclo de vida es Scoped, lo que significa que se crea una instancia
    /// por cada request HTTP y se comparte dentro del mismo request.
    /// </summary>
    private readonly AppDbContext _db;

    /// <summary>
    /// Constructor que recibe el DbContext mediante inyección de dependencias.
    /// </summary>
    /// <param name="db">Contexto de base de datos de la aplicación.</param>
    public OrderRepository(AppDbContext db) => _db = db;

    /// <summary>
    /// Obtiene una orden por su identificador único, incluyendo sus ítems.
    /// Usa Include para cargar los OrderItems en la misma consulta (eager loading),
    /// evitando el problema N+1 de consultas.
    /// </summary>
    /// <param name="id">Identificador único de la orden.</param>
    /// <param name="ct">Token de cancelación para operaciones asíncronas.</param>
    /// <returns>La orden encontrada, o null si no existe.</returns>
    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    /// <summary>
    /// Obtiene todas las órdenes incluyendo sus ítems.
    /// En producción se recomienda paginar este método para evitar
    /// cargar grandes volúmenes de datos en memoria.
    /// </summary>
    /// <param name="ct">Token de cancelación para operaciones asíncronas.</param>
    /// <returns>Lista de todas las órdenes con sus ítems.</returns>
    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default)
        => await _db.Orders
            .Include(o => o.Items)
            .ToListAsync(ct);

    /// <summary>
    /// Agrega una nueva orden al contexto de EF Core.
    /// La orden no se persiste hasta que se llame CommitAsync en el UnitOfWork.
    /// </summary>
    /// <param name="entity">La orden a agregar.</param>
    /// <param name="ct">Token de cancelación para operaciones asíncronas.</param>
    public async Task AddAsync(Order entity, CancellationToken ct = default)
        => await _db.Orders.AddAsync(entity, ct);

    /// <summary>
    /// Marca una orden como modificada en el contexto de EF Core.
    /// Los cambios no se persisten hasta que se llame CommitAsync en el UnitOfWork.
    /// </summary>
    /// <param name="entity">La orden con los cambios aplicados.</param>
    public void Update(Order entity)
        => _db.Orders.Update(entity);

    /// <summary>
    /// Marca una orden para ser eliminada en el contexto de EF Core.
    /// La eliminación no se persiste hasta que se llame CommitAsync en el UnitOfWork.
    /// </summary>
    /// <param name="entity">La orden a eliminar.</param>
    public void Delete(Order entity)
        => _db.Orders.Remove(entity);
}