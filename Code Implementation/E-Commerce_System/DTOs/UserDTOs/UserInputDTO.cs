using System.ComponentModel.DataAnnotations;

namespace E_Commerce_System.DTOs.UserDTOs
{
    public class UserInputDTO
    {
        [Required(ErrorMessage = "User name is required")]
        [StringLength(50, ErrorMessage = "User name must be 50 characters or less")]
        public string userName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [StringLength(50, ErrorMessage = "User email must be 50 characters or less")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email must be in this pattern: example@example.com")]
        public string userEmail { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be 8 to 20 characters")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must contain at least 1 letter, 1 number")]
        public string userPassword { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "number length must be between 8 to 20 characters")]
        public string userPhone { get; set; }

        [Required(ErrorMessage = "Role must be entered")]
        public string userRole { get; set; }
    }
}
