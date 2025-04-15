namespace LearnCSharp.Domain.Entities
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public float Total { get; set; }
        public string Color { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
    }
}