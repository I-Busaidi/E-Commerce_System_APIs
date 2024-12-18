using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_System.DTOs.OrderDTOs
{
    public class OrderInputDTO
    {
        public decimal orderTotalAmount { get; set; }

        public int userId { get; set; }
    }
}
