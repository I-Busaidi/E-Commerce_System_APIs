using E_Commerce_System.Models;

namespace E_Commerce_System.DTOs.OrderDTOs
{
    public class DetailedOrderDTO
    {
        public OrderOutputDTO order { get; set; }
        public List<OrderProductOutputDTO> orderProducts { get; set; }
    }
}
