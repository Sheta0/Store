using AutoMapper;
using Domain.Models.OrderModels;
using Shared.Dtos.OrderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAddress = Domain.Models.Identity.Address;
using OrderAddress = Domain.Models.OrderModels.Address;

namespace Services.Mapping_Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderAddress, AddressDto>().ReverseMap();
            CreateMap<UserAddress, AddressDto>().ReverseMap();

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.ProductInOrderItem.ProductId))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.ProductInOrderItem.ProductName))
                .ForMember(d => d.PictureUrl, o => o.MapFrom(s => s.ProductInOrderItem.PictureUrl));

            CreateMap<Order, OrderResultDto>()
                .ForMember(d => d.PaymentStatus, o => o.MapFrom(s => s.PaymentStatus.ToString()))
                .ForMember(d => d.DeliveryMethod, o => o.MapFrom(s => s.DeliveryMethod.ShortName))
                .ForMember(d => d.Total, o => o.MapFrom(s => s.DeliveryMethod.Cost + s.Subtotal));

            CreateMap<DeliveryMethod, DeliveryMethodDto>();

        }
    }
}
