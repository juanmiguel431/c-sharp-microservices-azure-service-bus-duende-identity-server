using Mango.Services.OrderApi.DbContexts;
using Mango.Services.OrderApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.OrderApi.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContext;

    public OrderRepository(DbContextOptions<ApplicationDbContext> dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<bool> AddOrder(OrderHeader orderHeader)
    {
        await using var _db = new ApplicationDbContext(_dbContext);
        _db.OrderHeaders.Add(orderHeader);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid)
    {
        await using var _db = new ApplicationDbContext(_dbContext);
        var orderHeader = await _db.OrderHeaders.FirstOrDefaultAsync(p => p.OrderHeaderId == orderHeaderId);
        if (orderHeader == null) return;
        orderHeader.PaymentStatus = paid;
        await _db.SaveChangesAsync();
    }
}