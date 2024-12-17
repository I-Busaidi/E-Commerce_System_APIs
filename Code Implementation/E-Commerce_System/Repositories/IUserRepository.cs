using E_Commerce_System.Models;

namespace E_Commerce_System.Repositories
{
    public interface IUserRepository
    {
        User AddUser(User user);
        void DeleteUser(User user);
        IEnumerable<User> GetAllUsers();
        User GetUserByEmail(string email);
        User GetUserById(int id);
        User UpdateUser(User user);
    }
}