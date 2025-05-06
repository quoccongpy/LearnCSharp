using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models;
using LearnCSharp.Application.Models.DTOs.Order;
using LearnCSharp.Application.Utility;
using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;
using System.Linq.Expressions;

namespace LearnCSharp.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;

        public OrderService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _userService = userService;
        }

        public async Task CreateAsync(OrderCreateDTO model)
        {
            try
            {
                var userId = _currentUserService.UserId;
                if (userId == Guid.Empty)
                {
                    throw new UnauthorizedAccessException();
                }
                var order = new Order()
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    Note = model.Note,
                    OrderDate = DateTime.UtcNow,
                    Status = SD.Pending,
                    ShippingMethod = model.ShippingMethod,
                    ShippingAddress = model.ShippingAddress,
                    PaymentMethod = model.PaymentMethod,
                    UserId = userId,
                    IsActive = true,
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

        public async Task DeleteAsync(int id)
        {
            try
            {
                var userId = _currentUserService.UserId;
                var user = await _userService.GetUserByIdAsync(userId);
                var order = await _unitOfWork.Order.GetByIdAsync(id);
                if (order.UserId != userId && !await _userService.IsUserInRoleAsync(userId, SD.RoleAdmin))
                {
                    throw new Exception("You do not have permission to delete this order.");
                }
                if (order.Status != SD.Pending)
                {
                    throw new Exception("Orders can only be canceled in pending status.");
                }
                await _unitOfWork.BeginTransactionAsync();

                order.Status = SD.Cancelled;
                order.IsActive = false;

                _unitOfWork.Order.Update(order);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<PagedResult<OrderDTO>> GetAllOrderPagingAsync(string? keyword, string? status, int pageIndex = 1, int pageSize = 10)
        {
            Expression<Func<Order, bool>> filter = a => (string.IsNullOrEmpty(keyword) || a.PhoneNumber.Contains(keyword))
                                                        && (!string.IsNullOrEmpty(keyword) || a.Status.Contains(status));
            var (order, totalCount) = await _unitOfWork.Order.GetPagedAsync(filter, ((pageIndex - 1) * pageSize), pageSize);
            var data = order.Select(a => new OrderDTO
            {
                Id = a.Id,
                Status = a.Status,
                FullName = a.FullName,
                PhoneNumber = a.PhoneNumber,
                Address = a.Address,
                Note = a.Note,
                OrderDate = a.OrderDate,
                TotalMoney = a.TotalMoney,
                PaymentMethod = a.PaymentMethod,
            }).ToList();

            var result = new PagedResult<OrderDTO>
            {
                Results = data,
                CurrentPage = pageIndex,
                RowCount = totalCount,
                PageSize = pageSize
            };
            return result;
        }

        public async Task<OrderDTO> GetOrderByIdAsync(int id)
        {
            var userId = _currentUserService.UserId;
            var user = await _userService.GetUserByIdAsync(userId);
            var order = await _unitOfWork.Order.GetByIdAsync(id);

            var data = new OrderDTO()
            {
                Id = order.Id,
                FullName = order.FullName,
                Email = order.Email,
                PhoneNumber = order.PhoneNumber,
                Address = order.Address,
                Note = order.Note,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalMoney = order.TotalMoney,
                ShippingAddress = order.ShippingAddress,
                PaymentMethod = order.PaymentMethod,
                UserName = user.UserName,
                OrderDetails = await GetOrderDetailsByOrderIdAsync(id),
            };
            return data;
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

        private async Task<List<OrderDetailDTO>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            var orderDetail = await _unitOfWork.OrderDetail.GetAllAsync(a => a.OrderId == orderId);

            var productIds = orderDetail.Select(x => x.ProductId).Distinct().ToList();

            var products = await _unitOfWork.Product.GetAllAsync(a => productIds.Contains(a.Id));

            var productDict = products.ToDictionary(p => p.Id);

            var data = new List<OrderDetailDTO>();
            foreach (var item in orderDetail)
            {
                if (productDict.TryGetValue(item.ProductId, out var product))
                {
                    data.Add(new OrderDetailDTO()
                    {
                        Id = item.Id,
                        //OrderId = item.OrderId,
                        //ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = product.Price,
                        ProductName = product.Name,
                        ProductThumbnail = product.Thumbnaill,
                    });
                }
            }
            return data;
        }
    }
}