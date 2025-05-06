namespace LearnCSharp.Application.Models.DTOs.Order
{
    public class OrderDetailDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductThumbnail { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        //public float SubTotal { get; set; }
    }
}