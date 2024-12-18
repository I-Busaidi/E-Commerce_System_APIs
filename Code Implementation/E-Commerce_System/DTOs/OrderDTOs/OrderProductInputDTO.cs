using E_Commerce_System.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce_System.DTOs.OrderDTOs
{
    public class OrderProductInputDTO
    {
        public int orderId { get; set; }

        public int productId { get; set; }

        public int productQuantity { get; set; }
    }
}
