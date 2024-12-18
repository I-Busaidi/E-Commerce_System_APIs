using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_System.DTOs.OrderDTOs
{
    public class OrderOutputDTO
    {
        public int orderId { get; set; }

        public DateTime orderDate { get; set; }

        public decimal orderTotalAmount { get; set; }

        public string userName { get; set; }
    }
}
