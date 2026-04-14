using ErpSystem.Domain.Entities;
using ErpSystem.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.Infrastructure.Persistence.Repositories;

public class OrderItemRepository : IRepository<OrderItem>
{
    private readonly AppDbContext _db;

    public OrderItemRepository(AppDbContext db) => _db = db;

    public async Task<OrderItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.OrderItems.FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<IReadOnlyList<OrderItem>> GetAllAsync(CancellationToken ct = default)
        => await _db.OrderItems.ToListAsync(ct);

    public async Task AddAsync(OrderItem entity, CancellationToken ct = default)
        => await _db.OrderItems.AddAsync(entity, ct);

    public void Update(OrderItem entity) { }

    public void Delete(OrderItem entity)
        => _db.OrderItems.Remove(entity);
}
