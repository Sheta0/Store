using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.OrderDtos
{
    public class OrderResultDto
    {
        // Id
        public Guid Id { get; set; } = Guid.NewGuid();

        // User Email
        public string UserEmail { get; set; }

        // Shipping Address
        public AddressDto ShippingAddress { get; set; }

        // Order Items
        public ICollection<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>(); // Navigational Property

        // Delivery Method
        public string DeliveryMethod { get; set; }

        // Payment Status
        public string PaymentStatus { get; set; }

        // Order Subtotal
        public decimal Subtotal { get; set; }

        // Order Date  
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;

        // Payment Intent
        public string PaymentIntentId { get; set; } = string.Empty;

        public decimal Total { get; set; }
    }
}
