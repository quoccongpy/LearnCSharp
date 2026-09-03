namespace LearnCSharp.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Thumbnail { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
        public ICollection<ProductVariant> ProductVariants { get; set; }
        public ICollection<ProductReview> Reviews { get; set; }
    }
}