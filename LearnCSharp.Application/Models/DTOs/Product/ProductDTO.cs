using LearnCSharp.Application.Models.DTOs.ProductImage;

namespace LearnCSharp.Application.Models.DTOs.Product
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Thumbnail { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<ProductImageDTO> ProductImagesList { get; set; }
    }
}