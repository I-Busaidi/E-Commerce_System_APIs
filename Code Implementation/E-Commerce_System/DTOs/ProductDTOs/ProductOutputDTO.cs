using System.ComponentModel.DataAnnotations;

namespace E_Commerce_System.DTOs.ProductDTOs
{
    public class ProductOutputDTO
    {
        public string productName { get; set; }

        public string? productDescription { get; set; }

        public decimal productPrice { get; set; }

        public int productStock { get; set; } 

        public decimal? productRating { get; set; }
    }
}
