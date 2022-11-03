using Mango.Services.OrderApi.Models;

namespace Mango.Services.OrderApi.Repositories;

public interface IOrderRepository
{
    Task<bool> AddOrder(OrderHeader orderHeader);
    Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid);
    
}