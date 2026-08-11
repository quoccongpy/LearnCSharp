using LearnCSharp.API.Hubs;
using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Notification;
using Microsoft.AspNetCore.SignalR;

namespace LearnCSharp.API.Services
{
    public class SignalRNotificationService : IRealtimeNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PushToUserAsync(Guid userId, NotificationDTO notification)
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", notification);
        }
    }
}
