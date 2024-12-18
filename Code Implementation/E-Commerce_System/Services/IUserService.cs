using E_Commerce_System.DTOs.UserDTOs;
using E_Commerce_System.Models;

namespace E_Commerce_System.Services
{
    public interface IUserService
    {
        (List<(string productName, int quantity, decimal productSum)> cart, decimal grandTotal) GetCartDetails(int id);
        UserOutputDTO AddUser(UserInputDTO userInputDTO);
        List<UserOutputDTO> GetAllUsers();
        List<User> GetAllUsersWithRelatedData();
        UserOutputDTO GetUserById(int id);
        User GetUserByIdWithRelatedData(int id);
        User GetUserByLoginCredentials(string email, string password);
        List<UserOutputDTO> GetUsersByRole(string role);
    }
}