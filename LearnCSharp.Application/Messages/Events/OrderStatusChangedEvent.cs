namespace LearnCSharp.Application.Messages.Events
{
    public record OrderStatusChangedEvent
        (
        int OrderId,
        Guid UserId,
        string OldStatus,
        string NewStatus,
        string NotificationTitle,
        string NotificationMessage,
        string NotificationType
    );
}