namespace LearnCSharp.Domain.Entities
{
    public class Cart
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}