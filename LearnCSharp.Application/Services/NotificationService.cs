using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Notification;
using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;

namespace LearnCSharp.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public NotificationService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task CreateAsync(Guid userId, NotificationDTO model)
        {
            var entity = new Notification
            {
                UserId = userId,
                Title = model.Title,
                Message = model.Message,
                Type = model.Type,
                OrderId = model.OrderId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
            };
            await _unitOfWork.Notification.CreateAsync(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var userId = _currentUserService.UserId;
            var entity = await _unitOfWork.Notification.GetByfilterAsync(n => n.Id == id && n.UserId == userId);
            if (entity != null)
            {
                await _unitOfWork.Notification.RemoveAsync(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<(List<NotificationDTO> Items, int TotalCount)> GetByUserAsync(int pageIndex, int pageSize)
        {
            var userId = _currentUserService.UserId;
            var (items, total) = await _unitOfWork.Notification.GetPagedAsync(filter: n => n.UserId == userId, skip: (pageIndex - 1) * pageSize, take: pageSize);
            var data = items.Select(n => new NotificationDTO
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                OrderId = n.OrderId,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
            }).ToList();
            return (data, total);
        }

        public async Task<int> GetUnreadCountAsync()
        {
            var userId = _currentUserService.UserId;
            return await _unitOfWork.Notification.GetUnreadCountAsync(userId);
        }

        public async Task MarkAllAsReadAsync()
        {
            var userId = _currentUserService.UserId;
            await _unitOfWork.Notification.MarkAllAsReadAsync(userId);
            await _unitOfWork.CompleteAsync();
        }

        public async Task MarkAsReadAsync(int id)
        {
            var userId = _currentUserService.UserId;
            await _unitOfWork.Notification.MarkAsReadAsync(id, userId);
            await _unitOfWork.CompleteAsync();
        }
    }
}