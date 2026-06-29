namespace LearnCSharp.API.Models
{
    public class ProductCreateRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public IFormFile? Thumbnail { get; set; }
        public IList<IFormFile>? Images { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
    }
}
