using Shared.Dtos.OrderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IOrderService
    {
        // Get Order By Id Async
        Task<OrderResultDto> GetOrderByIdAsync(Guid id);

        // Get All Orders for a User Async
        Task<IEnumerable<OrderResultDto>> GetOrdersByUserEmailAsync(string userEmail);

        // Create Order
        Task<OrderResultDto> CreateOrderAsync(OrderRequestDto orderRequestDto, string userEmail);

        // Get All Delivery Methods
        Task<IEnumerable<DeliveryMethodDto>> GetDeliveryMethodsAsync(); 
    }
}
