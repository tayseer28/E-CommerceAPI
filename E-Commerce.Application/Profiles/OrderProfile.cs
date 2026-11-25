using AutoMapper;
using E_Commerce.Application.DTOs.Authentications;
using E_Commerce.Application.DTOs.Orders;
using E_Commerce.Domain.Entities.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<AddressDto, OrderAddress>().ReverseMap();
         
            CreateMap<Order, OrderToReturnDto>()
                .ForMember(dest => dest.DeliveryMethod, O => O.MapFrom(src => src.DeliveryMethod.ShortName))
                .ForMember(dest => dest.DeliveryMethodCost, O => O.MapFrom(src => src.DeliveryMethod.Cost))
                .ForMember(dest => dest.Status, O => O.MapFrom(src => src.Status.ToString()))// auto mapper can map it automatically
                .ForMember(dest => dest.Total, O => O.MapFrom(src => src.GetTotal()));       // auto mapper can map it automatically

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductId, oi => oi.MapFrom(src => src.ProductItemOrdered.ProductId))
                .ForMember(dest => dest.ProductName, oi => oi.MapFrom(src => src.ProductItemOrdered.ProductName))
                .ForMember(dest => dest.PictureUrl, oi => oi.MapFrom<OrderItemPictureUrlResolver>());


            CreateMap<DeliveryMethod, DeliveryMethodDto>();

        }
    }
}
