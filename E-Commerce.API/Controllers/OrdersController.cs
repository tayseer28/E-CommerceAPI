using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    public class OrdersController : ApiBaseController
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // post create order => Order(basketId , shipping address) [by user email]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto orderDto, CancellationToken ct)
            => ToActionResult(await _orderService.CreateOrderAsync(orderDto, GetEmailFromToken(), ct));



        // GET Orders of User => [by email] Orders for Logged in user
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetAllOrders(CancellationToken ct)
            => ToActionResult(await _orderService.GetAllOrdersForUserAsync(GetEmailFromToken(), ct));

        // Get Order Of User => orderId + [email] 
        [Authorize]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderById(Guid id, CancellationToken ct)
             => ToActionResult(await _orderService.GetOrderForUserByIdAsync(id, GetEmailFromToken(), ct));


        // GET Delivery Method => List Of Delivery methods
        [AllowAnonymous] // means does not have to be authorized (default)
        [HttpGet("deliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethodDto>>> GetDeliveryMethods(CancellationToken ct)
            => ToActionResult(await _orderService.GetAllDeliveryMethodsAsync(ct));
    }
}
