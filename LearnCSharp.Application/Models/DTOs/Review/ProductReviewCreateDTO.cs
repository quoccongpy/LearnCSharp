namespace LearnCSharp.Application.Models.DTOs.Review
{
    public class ProductReviewCreateDTO
    {
        public int ProductId { get; set; }
        public int Rating { get; set; }       
        public string Comment { get; set; }
    }
}