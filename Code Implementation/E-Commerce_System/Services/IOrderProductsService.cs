using E_Commerce_System.DTOs.OrderDTOs;
using E_Commerce_System.Models;

namespace E_Commerce_System.Services
{
    public interface IOrderProductsService
    {
        OrderProductOutputDTO AddProducts(OrderProducts orderProductsInput);
    }
}