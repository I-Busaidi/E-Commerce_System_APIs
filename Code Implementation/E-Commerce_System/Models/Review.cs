using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_System.Models
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int reviewId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int rating { get; set; }

        [StringLength(300, ErrorMessage = "Review comment cannot be longer than 300 characters")]
        public string? comment { get; set; }

        public DateTime reviewDate { get; set; } = DateTime.Now;

        [ForeignKey("Reviewer")]
        public int userId { get; set; }
        public virtual User Reviewer { get; set; }

        [ForeignKey("ReviewedProduct")]
        public int productId { get; set; }
        public virtual Product ReviewedProduct { get; set; }
    }
}
