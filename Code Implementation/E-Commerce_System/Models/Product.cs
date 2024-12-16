using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_System.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int productId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(50, ErrorMessage = "Product name must be 50 characters or less")]
        public string productName { get; set; }

        public string? productDescription { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.001, int.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal productPrice { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be less than 0")]
        public int productStock { get; set; } = 0;

        public decimal? productRating { get; set; }

        [InverseProperty("Product")]
        public virtual ICollection<OrderProducts> ProductOrders { get; set; } = new List<OrderProducts>();

        [InverseProperty("ReviewedProduct")]
        public virtual ICollection<Review> ProductReviews { get; set; } = new List<Review>();
    }
}
