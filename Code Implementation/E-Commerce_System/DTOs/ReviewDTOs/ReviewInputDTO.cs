using E_Commerce_System.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce_System.DTOs.ReviewDTOs
{
    public class ReviewInputDTO
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int rating { get; set; }

        [StringLength(300, ErrorMessage = "Review comment cannot be longer than 300 characters")]
        public string? comment { get; set; }

        [Required]
        public int userId { get; set; }

        [Required]
        public int productId { get; set; }
    }
}
