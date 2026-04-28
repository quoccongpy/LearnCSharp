namespace LearnCSharp.Domain.Entities
{
    public class Size
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ProductVariant> ProductVariants { get; set; }
    }
}