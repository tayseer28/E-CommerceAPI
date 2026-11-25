using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Orders;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Baskets;
using E_Commerce.Domain.Entities.Orders;
using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IBasketRepository basketRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<OrderToReturnDto>> CreateOrderAsync(OrderDto orderDto, string email, CancellationToken ct = default)
        {

            // get order items (map the basket to order items)
            var basket = await _basketRepository.GetBasketAsync(orderDto.BasketId);
            if (basket is null)
                return Error.NotFound("Basket.NotFound", $"Basket With the Id {orderDto.BasketId} is Not Found");

            if (basket.Items.Count == 0)
                return Result<OrderToReturnDto>.Fail(Error.Validation("Basket.Empty", $"Can Not Create Order With Basket With Id {orderDto.BasketId}"));

            var OrderItems = new List<OrderItem>(basket.Items.Count);

            var ProductsIds = basket.Items.Select(x => x.Id).ToHashSet();

            var Products = (await _unitOfWork.GetRepository<Product, int>()
                .GetAllAsync(new ProductWithIdSpecefications(ProductsIds), ct)).ToDictionary(X => X.Id); // matched products of the basket item from the database 

            foreach (var item in basket.Items)
            {
                // get the product value 
                if (!Products.TryGetValue(item.Id, out var product))
                    return Error.NotFound("Product.NotFound", $"Product with Id {item.Id} is Not Found");
                var orderItem = new OrderItem()
                {
                    
                    Price = product.Price,
                    Quantity = item.Quantity,
                    ProductItemOrdered = new ProductItemOrdered ()
                    {
                        ProductId = product.Id,
                        PictureUrl = product.PictureUrl,
                        ProductName = product.Name
                    }
                };
                OrderItems.Add(orderItem);
            }
            // get shippings address (as the order address )
            var orderAdderss = _mapper.Map<OrderAddress>(orderDto.ShippingAddress);

            // get the deliveryMethod (validate it)
            var deliveryMethod = await _unitOfWork.GetRepository<DeliveryMethod, int>().GetByIdAsync(orderDto.DeliveryMethodId);
            if (deliveryMethod is null)
                return Error.NotFound("DeliveryMethod.NotFound", $"Delivery Method with Id{orderDto.DeliveryMethodId} not Found");

            // get subtotal
            var subTotal = OrderItems.Sum(x => x.Quantity * x.Price);

            var order = new Order(email, orderAdderss, deliveryMethod, OrderItems, subTotal);
            _unitOfWork.GetRepository<Order, Guid>().Add(order); // local
            var result = await _unitOfWork.SaveChangesAsync(ct);
            if (result == 0)
                return Error.Failure("Order.SaveFailed", "Cannot Create Order");
            else
            {
                await _basketRepository.DeleteBasketAsync(orderDto.BasketId, ct);
                return _mapper.Map<OrderToReturnDto>(order);

            }

        }

        public async Task<Result<IReadOnlyList<DeliveryMethodDto>>> GetAllDeliveryMethodsAsync(CancellationToken ct = default)
        {
            var deliveryMethods = await _unitOfWork.GetRepository<DeliveryMethod, int>().GetAllAsync(ct);
            if (deliveryMethods.Any())
                return Result<IReadOnlyList<DeliveryMethodDto>>.Ok(_mapper.Map<IReadOnlyList<DeliveryMethodDto>>(deliveryMethods));

            else
                return Error.NotFound("Delivery Methods Not Found", $"Failed To Fetch Delivery Methods");

        }

        public async Task<Result<IReadOnlyList<OrderToReturnDto>>> GetAllOrdersForUserAsync(string email, CancellationToken ct = default)
        {
            var orders = await _unitOfWork.GetRepository<Order, Guid>().GetAllAsync(new OrderSpecifications(email));
            if (orders.Any())
                return Result<IReadOnlyList<OrderToReturnDto>>.Ok(_mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders));

            else
                return Error.NotFound("Orders Not Found", $"No Orders Found For User With Email{email}");
        }

        public async Task<Result<OrderToReturnDto>> GetOrderForUserByIdAsync(Guid orderId, string email, CancellationToken ct = default)
        {
            var order = await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(new OrderSpecifications(orderId, email));
            if (order is not null)
                return Result<OrderToReturnDto>.Ok(_mapper.Map<OrderToReturnDto>(order));

            else
                return Error.NotFound("Order Not Found", $"No Order Found For User With Email{email} and Order Id {orderId}");
        }
    }
}
