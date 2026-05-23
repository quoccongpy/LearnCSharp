namespace LearnCSharp.Application.Models.DTOs.ProductVariant
{
    public class ProductVariantListItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string SizeName { get; set; }
        public string CrustName { get; set; }
        public float Price { get; set; }
    }
}