using E_Commerce_System.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce_System.DTOs.ReviewDTOs
{
    public class ReviewOutputDTO
    {
        public int rating { get; set; }

        public string? comment { get; set; }

        public DateTime reviewDate { get; set; }

        public string userName { get; set; }

        public string productName { get; set; }
    }
}
