namespace Domain.Models.OrderModels
{
    public class OrderItem : BaseEntity<Guid>
    {
        public OrderItem() { }
        public OrderItem(ProductInOrderItem productInOrderItem, int quantity, decimal price)
        {
            ProductInOrderItem = productInOrderItem;
            Quantity = quantity;
            Price = price;
        }

        public ProductInOrderItem ProductInOrderItem { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}