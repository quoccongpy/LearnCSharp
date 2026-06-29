namespace LearnCSharp.Domain.Entities
{
    public class ProductVariant
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int SizeId { get; set; }
        public Size Size { get; set; }

        public int CrustId { get; set; }
        public Crust Crust { get; set; }

        public decimal Price { get; set; }

        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}