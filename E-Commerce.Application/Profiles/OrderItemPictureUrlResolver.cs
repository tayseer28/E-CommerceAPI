using AutoMapper;
using AutoMapper.Execution;
using E_Commerce.Application.DTOs.Orders;
using E_Commerce.Domain.Entities.Orders;
using Microsoft.Extensions.Options;


namespace E_Commerce.Application.Profiles
{
    public class OrderItemPictureUrlResolver : IValueResolver<OrderItem, OrderItemDto, string>
    {
        private readonly UrlSettings _settings;
        public OrderItemPictureUrlResolver(IOptions<UrlSettings> options)
        {
            _settings = options.Value;
        }
        public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
        {
            //// source > Images/Products/image.jpg
            //// retuen > https://localhost:5001/Files/Images/Products/image.jpg
            //// get the baseUrl from the appsettings.json file
            //var baseUrl = _settings.BaseUrl.TrimEnd('/');
            //var path = source.ProductItemOrdered.PictureUrl.TrimStart('/');
            //return $"{baseUrl}/Files/{path}";
            var picture = source.ProductItemOrdered.PictureUrl;
            if (string.IsNullOrWhiteSpace(picture)) return string.Empty;     // new
            var baseUrl = _settings.BaseUrl.TrimEnd('/');                    // new
            return $"{baseUrl}/Files/{picture.TrimStart('/')}";
        }
    }
}
