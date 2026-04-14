using ErpSystem.Domain.Entities;
using ErpSystem.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.Infrastructure.Persistence.Repositories;

public class OrderRepository : IRepository<Order>
{
    private readonly AppDbContext _db;

    public OrderRepository(AppDbContext db) => _db = db;

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default)
        => await _db.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task AddAsync(Order entity, CancellationToken ct = default)
        => await _db.Orders.AddAsync(entity, ct);

    public void Update(Order entity)
    {
        _db.Orders.Attach(entity);
        _db.Entry(entity).Property(o => o.Status).IsModified = true;
        _db.Entry(entity).Property(o => o.UpdatedAt).IsModified = true;
    }

    public void Delete(Order entity)
        => _db.Orders.Remove(entity);
}
