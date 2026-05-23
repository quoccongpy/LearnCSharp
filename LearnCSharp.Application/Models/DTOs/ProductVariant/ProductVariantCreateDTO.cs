namespace LearnCSharp.Application.Models.DTOs.ProductVariant
{
    public class ProductVariantCreateDTO
    {
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int CrustId { get; set; }
        public float Price { get; set; }
    }
}