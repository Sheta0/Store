using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using Domain.Models.OrderModels;
using Services.Abstractions;
using Services.Specifications;
using Shared.Dtos.OrderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class OrderService(IMapper mapper, IBasketRepository basketRepository, IUnitOfWork unitOfWork) : IOrderService
    {
        public async Task<OrderResultDto> CreateOrderAsync(OrderRequestDto orderRequestDto, string userEmail)
        {
            // 1. Address
            var address = mapper.Map<Address>(orderRequestDto.ShipToAddress);

            // 2. Order Items => Basket
            var basket = await basketRepository.GetBasketAsync(orderRequestDto.BasketId);
            if (basket == null) throw new BasketNotFoundException(orderRequestDto.BasketId);

            var orderItems = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var product = await unitOfWork.GetRepository<Product, int>().GetByIdAsync(item.Id);
                if (product == null) throw new ProductNotFoundException(item.Id);

                var orderItem = new OrderItem(new ProductInOrderItem(product.Id, product.Name, product.PictureUrl), item.Quantity, product.Price);
                orderItems.Add(orderItem);
            }

            // 3. Get Delivery Method
            var deliveryMethod = await unitOfWork.GetRepository<DeliveryMethod, int>().GetByIdAsync(orderRequestDto.DeliveryMethodId);
            if (deliveryMethod == null) throw new DeliveryMethodNotFoundException(orderRequestDto.DeliveryMethodId);

            // 4. Calculate Subtotal
            var subtotal = orderItems.Sum(item => item.Price * item.Quantity);

            // 5. Create Payment Intent Id
            var spec = new OrderWithPaymentIntentSpecifications(basket.PaymentIntentId);

            var existingOrder = await unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(spec);

            if(existingOrder is not null)
                unitOfWork.GetRepository<Order, Guid>().Delete(existingOrder);

            // Create Order
            var order = new Order(userEmail, address, orderItems, deliveryMethod, subtotal, basket.PaymentIntentId);
            await unitOfWork.GetRepository<Order, Guid>().AddAsync(order);

            var count = await unitOfWork.SaveChangesAsync();
            if (count == 0) throw new OrderCreateBadRequestException();

            var result = mapper.Map<OrderResultDto>(order);
            return result;
        }

        public async Task<IEnumerable<DeliveryMethodDto>> GetDeliveryMethodsAsync()
        {
            var deliveryMethods = await unitOfWork.GetRepository<DeliveryMethod, int>().GetAllAsync();
            var result = mapper.Map<IEnumerable<DeliveryMethodDto>>(deliveryMethods);
            
            return result;
        }

        public async Task<OrderResultDto> GetOrderByIdAsync(Guid id)
        {
            var spec = new OrderSpecifications(id);
            var order = await unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(spec);
            if (order == null) throw new OrderNotFoundException(id);

            var result = mapper.Map<OrderResultDto>(order);
            return result;
        }

        public async Task<IEnumerable<OrderResultDto>> GetOrdersByUserEmailAsync(string userEmail)
        {
            var spec = new OrderSpecifications(userEmail);
            var orders = await unitOfWork.GetRepository<Order, Guid>().GetAllAsync(spec);

            var result = mapper.Map<IEnumerable<OrderResultDto>>(orders);
            return result;
        }
    }
}
