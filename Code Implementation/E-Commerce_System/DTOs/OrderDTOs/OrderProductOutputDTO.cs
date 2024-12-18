using E_Commerce_System.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce_System.DTOs.OrderDTOs
{
    public class OrderProductOutputDTO
    {
        public int orderId { get; set; }

        public string productName { get; set; }

        public decimal productPrice { get; set; }

        public int productQuantity { get; set; }
    }
}
