namespace E_Commerce.Server.Entity
{
    public class Order
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
