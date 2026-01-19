using Microsoft.AspNetCore.Http;

namespace LearnCSharp.Application.Models.DTOs.Product
{
    public class ProductCreateDTO
    {
        public string Name { get; set; }
        public float Price { get; set; }
        public IFormFile? Thumbnail { get; set; }
        public IList<IFormFile>? Images { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
    }
}