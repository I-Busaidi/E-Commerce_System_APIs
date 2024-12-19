using System.ComponentModel.DataAnnotations;

namespace E_Commerce_System.DTOs.ReviewDTOs
{
    public class ReviewInputWithProductNameDTO
    {
        [Required]
        public string productName { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int rating { get; set; }

        [StringLength(300, ErrorMessage = "Review comment cannot be longer than 300 characters")]
        public string? comment { get; set; }
    }
}
