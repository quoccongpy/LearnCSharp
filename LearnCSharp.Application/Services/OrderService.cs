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

        public async Task<int> CreateAsync(OrderCreateDTO model)
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
                    ScheduledTime = model.ScheduledTime,
                    UserId = userId,
                };
                decimal totalMoney = 0;
                int totalItem = 0;
                var orderDetails = new List<OrderDetails>();
                foreach (var item in model.OrderDetails)
                {
                    var product = await _unitOfWork.Product.GetByIdAsync(item.ProductId);
                    if (product == null)
                    {
                        throw new Exception($"Product with ID {item.ProductId} does not exist");
                    }
                    decimal unitPrice = product.Price;
                    string sizeName = null;
                    string crustName = null;
                    if (item.ProductVariantId.HasValue)
                    {
                        var variant = await _unitOfWork.ProductVariant.GetByIdAsync(item.ProductVariantId.Value);
                        if (variant != null)
                        {
                            unitPrice = variant.Price;
                            sizeName = item.SizeName;
                            crustName = item.CrustName;
                        }
                    }
                    var orderDetail = new OrderDetails()
                    {
                        ProductId = product.Id,
                        ProductVariantId = item.ProductVariantId,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        ProductName = product.Name,
                        SizeName = sizeName,
                        CrustName = crustName,
                        Note = item.Note,
                        Total = unitPrice * item.Quantity,
                    };
                    orderDetails.Add(orderDetail);
                    totalMoney += orderDetail.Total;
                    totalItem += item.Quantity;
                }
                order.TotalMoney = totalMoney;
                order.TotalItem = totalItem;

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
                return order.Id;
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
            Expression<Func<Order, bool>> filter = a =>(string.IsNullOrEmpty(keyword) || a.PhoneNumber.Contains(keyword) || a.FullName.Contains(keyword) || a.Id.ToString() == keyword)
                                                   && (string.IsNullOrEmpty(status) || a.Status == status);
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
            if (order == null)
            {
                throw new ApplicationException("No find Order.");
            }
            if (order.UserId != userId && !await _userService.IsUserInRoleAsync(userId, SD.RoleAdmin))
            {
                throw new UnauthorizedAccessException("You do not have permission to view this order.");
            }

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
                UserName = user.UserName,
                OrderDetails = await GetOrderDetailsByOrderIdAsync(id),
            };
            return data;
        }

        public async Task<PagedResult<OrderDTO>> GetOrdersByStatusAsyncPagingAsync(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            var userId = _currentUserService.UserId;
            if (userId == Guid.Empty)
            {
                throw new UnauthorizedAccessException();
            }

            Expression<Func<Order, bool>> filter = a => a.UserId == userId && (string.IsNullOrEmpty(keyword) || a.PhoneNumber.Contains(keyword)
                                                                                                             || a.FullName.Contains(keyword)
                                                                                                             || a.Id.ToString() == keyword);
            var (orders, totalCount) = await _unitOfWork.Order.GetPagedAsync(filter, ((pageIndex - 1) * pageSize), pageSize);
            var data = orders.Select(a => new OrderDTO
            {
                Id = a.Id,
                Status = a.Status,
                FullName = a.FullName,
                PhoneNumber = a.PhoneNumber,
                Address = a.Address,
                Note = a.Note,
                OrderDate = a.OrderDate,
                TotalMoney = a.TotalMoney,
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

        public async Task<PagedResult<OrderDTO>> GetOrdersByUserAsyncPagingAsync(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            var userId = _currentUserService.UserId;
            if (userId == Guid.Empty)
            {
                throw new UnauthorizedAccessException();
            }

            Expression<Func<Order, bool>> filter = a => a.UserId == userId && (string.IsNullOrEmpty(keyword) || a.PhoneNumber.Contains(keyword)
                                                                                                             || a.FullName.Contains(keyword)
                                                                                                             || a.Id.ToString() == keyword);
            var (orders, totalCount) = await _unitOfWork.Order.GetPagedAsync(filter, ((pageIndex - 1) * pageSize), pageSize);
            var data = orders.Select(a => new OrderDTO
            {
                Id = a.Id,
                Status = a.Status,
                FullName = a.FullName,
                PhoneNumber = a.PhoneNumber,
                Address = a.Address,
                Note = a.Note,
                OrderDate = a.OrderDate,
                TotalMoney = a.TotalMoney,
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

        public Task UpdateAsync(int id, OrderUpdateDTO model)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            try
            {
                var order = await _unitOfWork.Order.GetByIdAsync(id);
                if (order == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy đơn hàng.");
                }
                ValidateStatusTransition(order.Status, status);

                await _unitOfWork.BeginTransactionAsync();
                order.Status = status;
                _unitOfWork.Order.Update(order);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<List<OrderDetailDTO>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            var orderDetails = await _unitOfWork.OrderDetail.GetAllAsync(a => a.OrderId == orderId);
            var data = new List<OrderDetailDTO>();
            foreach (var item in orderDetails)
            {
                data.Add(new OrderDetailDTO()
                {
                    Id = item.Id,
                    OrderId = item.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.UnitPrice,
                    ProductName = item.ProductName,
                    SizeName = item.SizeName,
                    CrustName = item.CrustName,
                    Note = item.Note,
                    ProductVariantId = item.ProductVariantId
                });
            }
            return data;
        }

        private static void ValidateStatusTransition(string currentStatus, string newStatus)
        {
            var validTransitions = new Dictionary<string, string[]>
            {
                {
                    SD.Pending, new[]{SD.Confirmed,SD.Cancelled,SD.Rejected}
                },
                {
                    SD.Confirmed,new[]{SD.Preparing,SD.Cancelled}
                },
                {
                    SD.Preparing,new[]{SD.ReadyForPickup,SD.Cancelled}
                },
                {
                    SD.ReadyForPickup, new[]{SD.Delivering}
                },
                { SD.Delivering, new[]{SD.Delivered}
                }
            };
            if (!validTransitions.TryGetValue(currentStatus, out var nextStatuses) ||
                !nextStatuses.Contains(newStatus))
            {
                throw new InvalidOperationException(
                    $"Cannot change status from '{currentStatus}' to '{newStatus}'.");
            }
        }
    }
}