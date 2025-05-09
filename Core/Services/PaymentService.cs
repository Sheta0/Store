using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using Domain.Models.OrderModels;
using Microsoft.Extensions.Configuration;
using Services.Abstractions;
using Services.Specifications;
using Shared.Dtos;
using Stripe;
using Stripe.Forwarding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using OrderProduct = Domain.Models.Product;

namespace Services
{
    public class PaymentService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper) : IPaymentService
    {
        public async Task<BasketDto> CreateOrUpdatePaymentIntentAsync(string basketId)
        {
            var basket = await basketRepository.GetBasketAsync(basketId);
            if (basket is null) throw new BasketNotFoundException(basketId);

            foreach (var item in basket.Items)
            {
                var product = await unitOfWork.GetRepository<OrderProduct, int>().GetByIdAsync(item.Id);
                if (product is null) throw new ProductNotFoundException(item.Id);

                item.Price = product.Price;
            }
            if (!basket.DeliveryMethodId.HasValue) throw new DeliveryMethodNotFoundException(-1);

            var deliveryMethod = await unitOfWork.GetRepository<DeliveryMethod, int>().GetByIdAsync(basket.DeliveryMethodId.Value);
            if (deliveryMethod is null) throw new DeliveryMethodNotFoundException(basket.DeliveryMethodId.Value);

            basket.ShippingPrice = deliveryMethod.Cost;

            var amount = (long)(basket.Items.Sum(item => item.Price * item.Quantity) + basket.ShippingPrice) * 100;

            StripeConfiguration.ApiKey = configuration["StripeSettings:SecretKey"];

            var service = new PaymentIntentService();

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                // Create
                var createOptions = new PaymentIntentCreateOptions()
                {
                    Amount = amount,
                    Currency = "USD",
                    PaymentMethodTypes = new List<string> { "card" },
                };

                var paymentIntent = await service.CreateAsync(createOptions);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else
            {
                // Update
                var updateOptions = new PaymentIntentUpdateOptions()
                {
                    Amount = amount,
                };

                await service.UpdateAsync(basket.PaymentIntentId, updateOptions);
            }

            await basketRepository.UpdateBasketAsync(basket);

            var result = mapper.Map<BasketDto>(basket);
            return result;
        }

        public async Task UpdateOrderPaymentStatus(string request, string header)
        {
            var endpointSecret = configuration["StripeSettings:EndPointSecret"];
            var stripeEvent = EventUtility.ConstructEvent(request, header, endpointSecret, throwOnApiVersionMismatch:false);

            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

            // Handle the event
            switch (stripeEvent.Type)
            {
                case EventTypes.PaymentIntentSucceeded:
                    {
                        await UpdatePaymentSucceeded(paymentIntent!.Id);
                        break;
                    }

                case EventTypes.PaymentIntentPaymentFailed:
                    {
                        await UpdatePaymentFailed(paymentIntent!.Id);
                        break;
                    }
                // ... handle other event types
                default:
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                    break;
            }

        }

        private async Task UpdatePaymentFailed(string paymentIntentId)
        {
            var orderRepo = unitOfWork.GetRepository<Order, Guid>();
            var order = await orderRepo.GetByIdAsync(new OrderWithPaymentIntentSpecifications(paymentIntentId));
            if (order is null) throw new Exception("Order not found");
           
            order.PaymentStatus = OrderPaymentStatus.PaymentFailed;
            orderRepo.Update(order);
            await unitOfWork.SaveChangesAsync();
        }

        private async Task UpdatePaymentSucceeded(string paymentIntentId)
        {
            var orderRepo = unitOfWork.GetRepository<Order, Guid>();
            var order = await orderRepo.GetByIdAsync(new OrderWithPaymentIntentSpecifications(paymentIntentId));
            if (order is null) throw new Exception("Order not found");

            order.PaymentStatus = OrderPaymentStatus.PaymentRecieved;
            orderRepo.Update(order);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
