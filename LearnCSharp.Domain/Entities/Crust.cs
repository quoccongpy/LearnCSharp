namespace LearnCSharp.Domain.Entities
{
    public class Crust
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ProductVariant> ProductVariants { get; set; }
    }
}