using E_Commerce_System.Models;

namespace E_Commerce_System.DTOs.OrderDTOs
{
    public class CartDTO
    {
        public string productName { get; set; }

        public int quantity { get; set; }

        public decimal price { get; set; }

    }
}
