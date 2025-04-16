using Microsoft.AspNetCore.Http;

namespace LearnCSharp.Application.Models.DTOs.Product
{
    public class ProductUpdateDTO
    {
        public string? Name { get; set; }
        public float? Price { get; set; }
        public IFormFile? Thumbnaill { get; set; }
        public string? Description { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CategoryId { get; set; }
        public IList<IFormFile>? Image { get; set; }
    }
}