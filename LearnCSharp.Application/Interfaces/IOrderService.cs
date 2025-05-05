using LearnCSharp.Application.Models;
using LearnCSharp.Application.Models.DTOs.Order;

namespace LearnCSharp.Application.Interfaces
{
    public interface IOrderService
    {
        Task CreateAsync(OrderCreateDTO model, Guid userId);

        Task UpdateAsync(int id, OrderUpdateDTO model);

        Task<OrderDTO> GetOrderByIdAsync(int id);

        Task<PagedResult<OrderDTO>> GetAllOrderPagingAsync(string? keyword, int pageIndex = 1, int pageSize = 10);

        Task<PagedResult<OrderDTO>> GetOrdersByUserAsyncPagingAsync(string? keyword, int pageIndex = 1, int pageSize = 10);

        Task<PagedResult<OrderDTO>> GetOrdersByStatusAsyncPagingAsync(string? keyword, int pageIndex = 1, int pageSize = 10);

        Task DeleteAsync(int id);
    }
}