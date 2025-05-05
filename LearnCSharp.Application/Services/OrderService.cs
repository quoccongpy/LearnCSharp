using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models;
using LearnCSharp.Application.Models.DTOs.Order;
using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Enums;
using LearnCSharp.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LearnCSharp.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public OrderService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task CreateAsync(OrderCreateDTO model)
        {
            try
            {
                var userId = _currentUserService.UserId;
                if (userId == null)
                {
                    throw new Exception("User does not exist");
                }
                var order = new Order()
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    Note = model.Note,
                    OrderDate = DateTime.Now,
                    Status = OrderStatus.Pending,
                    ShippingMethod = model.ShippingMethod,
                    ShippingAddress = model.ShippingAddress,
                    PaymentMethod = model.PaymentMethod,
                    UserId = userId
                };
                float totalMoney = 0;
                var orderDetails = new List<OrderDetails>();
                foreach (var item in model.OrderDetails)
                {
                    var product = await _unitOfWork.Product.GetByIdAsync(item.ProductId);
                    if (product == null)
                    {
                        throw new Exception($"Product with ID {item.ProductId} does not exist");
                    }
                    //if (product.StockQuantity < item.Quantity)
                    //{
                    //    throw new Exception($"Product '{product.Name}' has only {product.StockQuantity} products left in stock");
                    //}
                    var orderDetail = new OrderDetails()
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        Price = product.Price,
                        Total = product.Price * item.Quantity,
                        //Color = item.Color  
                    };
                    orderDetails.Add(orderDetail);
                    totalMoney += orderDetail.Total;
                    //product.StockQuantity -= item.Quantity;
                }
                order.TotalMoney = totalMoney;

                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.Order.CreateAsync(order);
                await _unitOfWork.CompleteAsync();

                foreach (var detail in orderDetails)
                {
                    detail.OrderId = order.Id;
                    await _unitOfWork.OrderDetail.CreateAsync(detail);
                }
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
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