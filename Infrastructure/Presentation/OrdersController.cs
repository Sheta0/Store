using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.Dtos.OrderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController(IServiceManager serviceManager) : ControllerBase
    {
        [HttpPost] // POST: api/orders
        public async Task<IActionResult> CreateOrder(OrderRequestDto orderRequestDto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await serviceManager.OrderService.CreateOrderAsync(orderRequestDto, userEmail);

            return Ok(result);
        }

        [HttpGet] // GET: api/orders
        public async Task<IActionResult> GetOrders()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await serviceManager.OrderService.GetOrdersByUserEmailAsync(userEmail);
            return Ok(result);
        }

        [HttpGet("{id}")] // GET: api/orders/{id}
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var result = await serviceManager.OrderService.GetOrderByIdAsync(id);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("deliveryMethods")] // GET: api/orders/deliveryMethods
        public async Task<IActionResult> GetDeliveryMethods()
        {
            var result = await serviceManager.OrderService.GetDeliveryMethodsAsync();
            return Ok(result);
        }
    }
}
