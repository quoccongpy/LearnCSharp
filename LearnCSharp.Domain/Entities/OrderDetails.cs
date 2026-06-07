namespace LearnCSharp.Domain.Entities
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int? ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }
        public string ProductName { get; set; }
        public string SizeName { get; set; }
        public string CrustName { get; set; }
        public string Note { get; set; }
        public double UnitPrice { get; set; }
    }
}