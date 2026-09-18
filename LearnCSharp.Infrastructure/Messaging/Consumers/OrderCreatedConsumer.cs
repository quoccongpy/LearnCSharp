using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Messages.Events;
using LearnCSharp.Application.Models.DTOs.Notification;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LearnCSharp.Infrastructure.Messaging.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IRealtimeNotificationService _realtimeNotification;
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(INotificationService notificationService,IRealtimeNotificationService realtimeNotification,ILogger<OrderCreatedConsumer> logger)
        {
            _notificationService = notificationService;
            _realtimeNotification = realtimeNotification;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var evt = context.Message;
            _logger.LogInformation("Xử lý OrderCreatedEvent: OrderId={OrderId}, UserId={UserId}",evt.OrderId, evt.UserId);
            var notification = new NotificationDTO
            {
                Title = "Đặt hàng thành công",
                Message = $"Đơn hàng #{evt.OrderId} của bạn đã được đặt thành công. " +$"Tổng tiền: {evt.TotalMoney:N0} VNĐ",
                Type = "success",
                OrderId = evt.OrderId,
            };
            await _notificationService.CreateAsync(evt.UserId, notification);
            await _realtimeNotification.PushToUserAsync(evt.UserId, notification);
            _logger.LogInformation("Đã gửi notification cho OrderId={OrderId}", evt.OrderId);
        }
    }
}