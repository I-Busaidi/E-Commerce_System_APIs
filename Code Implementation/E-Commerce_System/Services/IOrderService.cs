using E_Commerce_System.DTOs.OrderDTOs;
using E_Commerce_System.Models;

namespace E_Commerce_System.Services
{
    public interface IOrderService
    {
        (string productName, int quantity, decimal productSum) AddItemToCart(int userId, string productName, int quantity);
        OrderOutputDTO AddOrder(OrderInputDTO orderInputDTO);
        (OrderOutputDTO order, List<OrderProductOutputDTO> orderProducts) ConfirmCheckout(int userId);
        (OrderOutputDTO order, List<OrderProductOutputDTO> orderProducts) GetOrderDetails(int orderId, int userId);
        List<OrderOutputDTO> GetUserOrders(int id);
        List<Order> GetUserOrdersWithRelatedData(int id);
    }
}