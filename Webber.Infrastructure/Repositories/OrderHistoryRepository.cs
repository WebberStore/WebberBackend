using Webber.Domain.Entities;
using Webber.Domain.Interfaces;
using Webber.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Webber.Infrastructure.Repositories;

public class OrderHistoryRepository(WebberDbContext context) : GenericRepository<OrderHistory>(context), IOrderHistoryRepository
{
    /// <summary>
    /// Retrieves a collection of OrderHistory records associated with a specific order.
    /// </summary>
    /// <param name="orderId">The ID of the order to retrieve the history for.</param>
    /// <returns>An IEnumerable of OrderHistory objects representing the order history.</returns>
    public async Task<IEnumerable<OrderHistory>> GetHistoryForOrderAsync(int orderId)
    {
        return await Context.OrderHistory
            .Where(oh => oh.OrderId == orderId)
            .OrderByDescending(oh => oh.CreatedAt)
            .ToListAsync();
    }
}