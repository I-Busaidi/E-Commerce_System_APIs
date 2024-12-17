using System.ComponentModel.DataAnnotations;

namespace E_Commerce_System.DTOs.ProductDTOs
{
    public class ProductInputDTO
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(50, ErrorMessage = "Product name must be 50 characters or less")]
        public string productName { get; set; }

        public string? productDescription { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.001, int.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal productPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be less than 0")]
        public int productStock { get; set; } = 0;
    }
}
