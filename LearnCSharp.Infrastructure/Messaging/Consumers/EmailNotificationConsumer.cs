using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Messages.Events;
using LearnCSharp.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LearnCSharp.Infrastructure.Messaging.Consumers
{
    public class EmailNotificationConsumer : IConsumer<OrderCreatedEvent>, IConsumer<OrderStatusChangedEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EmailNotificationConsumer> _logger;

        public EmailNotificationConsumer(
            IEmailService emailService,
            IUnitOfWork unitOfWork,
            ILogger<EmailNotificationConsumer> logger)
        {
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var testEmail = "thinasin73nguoilao@gmail.com";
            var evt = context.Message;
            _logger.LogInformation("EmailConsumer: Gửi email tạo đơn #{OrderId} tới {Email}",evt.OrderId, evt.Email);
            var html = EmailTemplates.OrderCreated(evt.FullName,evt.OrderId,evt.TotalMoney, evt.OrderDate);
            //await _emailService.SendAsync(toEmail: evt.Email,toName: evt.FullName,subject: $"[LearnCSharp Shop] Đặt hàng thành công - Đơn #{evt.OrderId:D6}",htmlBody: html);
            await _emailService.SendAsync(toEmail: testEmail, toName: evt.FullName,subject: $"[LearnCSharp Shop] Đặt hàng thành công - Đơn #{evt.OrderId:D6}",htmlBody: html);
        }

        public async Task Consume(ConsumeContext<OrderStatusChangedEvent> context)
        {
            var evt = context.Message;
            var order = await _unitOfWork.Order.GetByIdAsync(evt.OrderId);
            if (order == null || string.IsNullOrEmpty(order.Email))
            {
                _logger.LogWarning("EmailConsumer: Không tìm thấy email cho OrderId={OrderId}", evt.OrderId);
                return;
            }
            _logger.LogInformation("EmailConsumer: Gửi email đổi trạng thái #{OrderId} → {Status}",evt.OrderId, evt.NewStatus);
            var html = EmailTemplates.OrderStatusChanged(order.FullName,evt.OrderId,evt.NewStatus,evt.NotificationMessage);
            await _emailService.SendAsync(toEmail: order.Email,toName: order.FullName,subject: $"[LearnCSharp Shop] {evt.NotificationTitle} - Đơn #{evt.OrderId:D6}",htmlBody: html);
        }
    }
    }
}