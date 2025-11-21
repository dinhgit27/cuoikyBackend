namespace btapbackend.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending";
        public decimal TotalAmount { get; set; }

        public List<OrderDetail> OrderDetails { get; set; } = new();
    }
}