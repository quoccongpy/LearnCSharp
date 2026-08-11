using LearnCSharp.Application.Models.DTOs.Notification;

namespace LearnCSharp.Application.Interfaces
{
    public interface INotificationService
    {
        Task CreateAsync(Guid userId, NotificationDTO dto);

        Task<(List<NotificationDTO> Items, int TotalCount)> GetByUserAsync(int pageIndex, int pageSize);

        Task<int> GetUnreadCountAsync();

        Task MarkAsReadAsync(int id);

        Task MarkAllAsReadAsync();

        Task DeleteAsync(int id);
    }
}