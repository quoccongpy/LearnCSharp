namespace LearnCSharp.Application.Messages.Events
{
    public record OrderCreatedEvent
    (
        int OrderId,
        Guid UserId,
        string FullName,
        string Email,
        decimal TotalMoney,
        DateTime OrderDate
    );
}