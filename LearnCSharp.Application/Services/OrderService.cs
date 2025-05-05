using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models;
using LearnCSharp.Application.Models.DTOs.Order;
using LearnCSharp.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LearnCSharp.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task CreateAsync(OrderCreateDTO model, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<OrderDTO>> GetAllOrderPagingAsync(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<OrderDTO> GetOrderByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<OrderDTO>> GetOrdersByStatusAsyncPagingAsync(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<OrderDTO>> GetOrdersByUserAsyncPagingAsync(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(int id, OrderUpdateDTO model)
        {
            throw new NotImplementedException();
        }
    }
}