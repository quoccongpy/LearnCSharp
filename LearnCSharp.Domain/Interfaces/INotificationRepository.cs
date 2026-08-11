using LearnCSharp.Domain.Entities;

namespace LearnCSharp.Domain.Interfaces
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task MarkAsReadAsync(int id, Guid userId);
        Task MarkAllAsReadAsync(Guid userId);
        Task<int> GetUnreadCountAsync(Guid userId);
    }
}