using AutoMapper;
using E_Commerce_System.DTOs.ProductDTOs;
using E_Commerce_System.DTOs.UserDTOs;
using E_Commerce_System.Models;
using E_Commerce_System.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace E_Commerce_System.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public (List<(string productName, int quantity, decimal productSum)> cart,decimal grandTotal) GetCartDetails(int id)
        {
            List<(string, int, decimal)> cart = new List<(string, int, decimal)>();
            decimal grandTotal = 0;
            User user = _userRepository.GetUserById(id);
            if (user == null)
            {
                throw new KeyNotFoundException("Could not find user");
            }

            if (user.userCart.Count == 0)
            {
                throw new InvalidOperationException("User cart is empty");
            }

            foreach (var item in user.userCart)
            {
                cart.Add((item.product.productName, item.quantity, item.quantity * item.product.productPrice));
                grandTotal += item.quantity * item.product.productPrice;
            }

            return (cart, grandTotal);
        }

        public List<UserOutputDTO> GetAllUsers()
        {
            List<User> users = _userRepository.GetAllUsers()
                .OrderBy(u => u.userName)
                .ToList();
            if (users == null || users.Count == 0)
            {
                throw new InvalidOperationException("No users found");
            }

            List<UserOutputDTO> userOutputDTOs = _mapper.Map<List<UserOutputDTO>>(users);
            return userOutputDTOs;
        }

        public List<User> GetAllUsersWithRelatedData()
        {
            List<User> users = _userRepository.GetAllUsers()
                .OrderBy(u => u.userName)
                .ToList();
            if (users == null || users.Count == 0)
            {
                throw new InvalidOperationException("No users found");
            }
            return users;
        }

        public UserOutputDTO GetUserById(int id)
        {
            User user = _userRepository.GetUserById(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            UserOutputDTO userOutputDTO = _mapper.Map<UserOutputDTO>(user);
            return userOutputDTO;
        }

        public User GetUserByIdWithRelatedData(int id)
        {
            User user = _userRepository.GetUserById(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            return user;
        }

        public UserOutputDTO AddUser(UserInputDTO userInputDTO)
        {
            if (string.IsNullOrWhiteSpace(userInputDTO.userName))
            {
                throw new ArgumentNullException("User name is required");
            }

            if (string.IsNullOrWhiteSpace(userInputDTO.userEmail))
            {
                throw new ArgumentNullException("Email is required");
            }

            if (string.IsNullOrWhiteSpace(userInputDTO.userPassword))
            {
                throw new ArgumentNullException("Password is required");
            }

            if (!Regex.IsMatch(userInputDTO.userPassword, @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,}$"))
            {
                throw new InvalidDataException("Password must be at least 8 characters and at least contain 1 letter and 1 number");
            }

            if (!Regex.IsMatch(userInputDTO.userEmail, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                throw new InvalidDataException("Email format must be example@example.com");
            }

            if (string.IsNullOrWhiteSpace(userInputDTO.userPhone))
            {
                throw new ArgumentNullException("User phone is required");
            }

            if (userInputDTO.userPhone.Length < 8 || userInputDTO.userPhone.Length > 20)
            {
                throw new ArgumentOutOfRangeException("Phone number must be between 8 to 20 characters");
            }

            if (userInputDTO.userRole.ToLower().Trim() != "normal user" && userInputDTO.userRole.ToLower().Trim() != "admin")
            {
                throw new InvalidDataException("User role must be normal user or admin");
            }

            var checkExistingEmail = _userRepository.GetAllUsers()
                .FirstOrDefault(u => u.userEmail == userInputDTO.userEmail);

            if (checkExistingEmail != null)
            {
                throw new InvalidOperationException("User with this email already exists");
            }

            string hashedPassword = PasswordHasher.GetHashedPassword(userInputDTO.userPassword);

            User user = _mapper.Map<User>(userInputDTO);
            user.userPassword = hashedPassword;

            UserOutputDTO userOutputDTO = _mapper.Map<UserOutputDTO>(_userRepository.AddUser(user));

            return userOutputDTO;
        }

        public List<UserOutputDTO> GetUsersByRole(string role)
        {
            var usersDtos = GetAllUsers()
                .Where(u => u.userRole == role)
                .ToList();

            if (usersDtos.Count == 0)
            {
                throw new InvalidOperationException($"No users with \"{role}\" role found");
            }

            return usersDtos;
        }

        public User GetUserByLoginCredentials(string email, string password)
        {
            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                throw new KeyNotFoundException("Invalid email");
            }
            else
            {
                if (!PasswordHasher.GetPasswordVerification(password, user.userPassword))
                {
                    throw new InvalidDataException("Incorrect Password");
                }
                else
                {
                    return user;
                }
            }
        }
    }
}
