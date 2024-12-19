using Azure.Core;
using E_Commerce_System.DTOs.OrderDTOs;
using E_Commerce_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_System.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[Controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IJwtService _jwtService;

        public OrderController(IOrderService orderService, IJwtService jwtService)
        {
            _orderService = orderService;
            _jwtService = jwtService;
        }

        [HttpPost("PlaceOrder")]
        public IActionResult PlaceOrder()
        {
            try
            {
                int userId = _jwtService.DecodeToken(GetToken()).userId;
                var orderDetails = _orderService.ConfirmCheckout(userId);
                return Created(string.Empty, $"Order: \n{orderDetails.order} \nDetails:\n{orderDetails.orderProducts}");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("AddItemToCart/{name}/{quantity}")]
        public IActionResult AddItemToCart(string name, int quantity)
        {
            try
            {
                int userId = _jwtService.DecodeToken(GetToken()).userId;
                var itemInfo = _orderService.AddItemToCart(userId, name, quantity);
                return Created(string.Empty, $"Product name: {itemInfo.productName}\nQuantity: {itemInfo.quantity}\nTotal Cost: {itemInfo.productSum}");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetUserOrders")]
        public IActionResult GetUserOrders()
        {
            try
            {
                int userId = _jwtService.DecodeToken(GetToken()).userId;
                var userOrders = _orderService.GetUserOrders(userId);
                return Ok(userOrders);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetOrderDetails/{orderId}")]
        public IActionResult GetOrderDetails(int orderId)
        {
            try
            {
                int userId = _jwtService.DecodeToken(GetToken()).userId;
                var order = _orderService.GetOrderDetails(orderId, userId);
                var orderDetails = new DetailedOrderDTO
                {
                    order = order.order,
                    orderProducts = order.orderProducts
                };
                return Ok(orderDetails);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [NonAction]
        public string GetToken()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Token unavailable or expired");
            }

            return token;
        }
    }
}
