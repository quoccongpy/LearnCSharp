using LearnCSharp.Application.Models.DTOs.Notification;

namespace LearnCSharp.Application.Interfaces
{
    public interface IRealtimeNotificationService
    {
        Task PushToUserAsync(Guid userId, NotificationDTO notification);
    }
}