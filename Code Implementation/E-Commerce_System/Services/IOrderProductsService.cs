using E_Commerce_System.DTOs.OrderDTOs;

namespace E_Commerce_System.Services
{
    public interface IOrderProductsService
    {
        List<OrderProductOutputDTO> AddProducts(List<OrderProductInputDTO> orderProductsInputDTO);
    }
}