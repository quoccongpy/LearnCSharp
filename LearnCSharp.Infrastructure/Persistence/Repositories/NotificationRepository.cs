using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearnCSharp.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        private readonly LearnCSharpDbContext _context;

        public NotificationRepository(LearnCSharpDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _context.Notification.CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var notifications = await _context.Notification.Where(n => n.UserId == userId && !n.IsRead).ToListAsync();
            foreach (var n in notifications)
            {
                n.IsRead = true;
            }
            _context.UpdateRange(notifications);
        }
        public async Task MarkAsReadAsync(int id, Guid userId)
        {
            var notification = await _context.Notification.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.Update(notification);
            }
        }
    }
}