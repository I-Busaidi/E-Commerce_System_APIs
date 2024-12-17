using System.ComponentModel.DataAnnotations;

namespace E_Commerce_System.DTOs.UserDTOs
{
    public class UserOutputDTO
    {
        public string userName { get; set; }

        public string userEmail { get; set; }

        public string userPhone { get; set; }

        public string userRole { get; set; }
    }
}
