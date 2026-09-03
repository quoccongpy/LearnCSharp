namespace LearnCSharp.Application.Models.DTOs.Review
{
    public class ProductReviewDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public bool IsHidden { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}