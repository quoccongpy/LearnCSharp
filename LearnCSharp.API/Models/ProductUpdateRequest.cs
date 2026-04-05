namespace LearnCSharp.API.Models
{
    public class ProductUpdateRequest
    {
        public string? Name { get; set; }
        public float? Price { get; set; }
        public IFormFile? Thumbnail { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public IList<IFormFile>? Images { get; set; }
        public List<int>? ListRetainIdsImage { get; set; }
    }
}
