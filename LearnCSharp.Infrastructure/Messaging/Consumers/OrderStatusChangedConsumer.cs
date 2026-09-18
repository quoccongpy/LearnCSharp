using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Messages.Events;
using LearnCSharp.Application.Models.DTOs.Notification;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LearnCSharp.Infrastructure.Messaging.Consumers
{
    public class OrderStatusChangedConsumer : IConsumer<OrderStatusChangedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IRealtimeNotificationService _realtimeNotification;
        private readonly ILogger<OrderStatusChangedConsumer> _logger;
        public OrderStatusChangedConsumer(INotificationService notificationService,IRealtimeNotificationService realtimeNotification,ILogger<OrderStatusChangedConsumer> logger)
        {
            _notificationService = notificationService;
            _realtimeNotification = realtimeNotification;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<OrderStatusChangedEvent> context)
        {
            var evt = context.Message;
            _logger.LogInformation("Xử lý OrderStatusChangedEvent: OrderId={OrderId}, {Old}→{New}",evt.OrderId, evt.OldStatus, evt.NewStatus);

            var notification = new NotificationDTO
            {
                Title = evt.NotificationTitle,
                Message = evt.NotificationMessage,
                Type = evt.NotificationType,
                OrderId = evt.OrderId,
            };
            await _notificationService.CreateAsync(evt.UserId, notification);
            await _realtimeNotification.PushToUserAsync(evt.UserId, notification);
            _logger.LogInformation("Đã gửi notification đổi trạng thái OrderId={OrderId}", evt.OrderId);
        }
    }
}