using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_System.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int userId { get; set; }

        [Required(ErrorMessage = "User name is required")]
        [StringLength(50, ErrorMessage = "User name must be 50 characters or less")]
        public string userName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [StringLength(50, ErrorMessage = "User email must be 50 characters or less")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email must be in this pattern: example@example.com")]
        public string userEmail { get; set; }

        [Required]
        public string userPassword { get; set; } // hashed password

        [Required(ErrorMessage = "Phone number is required")]
        public string userPhone { get; set; }

        [Required(ErrorMessage = "Role must be entered")]
        public string userRole { get; set; }

        public DateTime createdAt { get; set; } = DateTime.Now;

        public DateTime? updatedAt { get; set; }

        public bool isActive { get; set; } = true;

        [ForeignKey("admin")]
        public int? updatedByAdminId { get; set; }
        public virtual User? admin { get; set; }

        [InverseProperty("Buyer")]
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        [InverseProperty("Reviewer")]
        public virtual ICollection<Review> ReviewsByUser { get; set; } = new List<Review>();

        [NotMapped]
        public List<(Product product, int quantity)>? userCart { get; set; } = new List<(Product product, int quantity)>();
    }
}
