using AutoMapper; // AutoMapper is used to map between domain models and DTOs.
using E_Commerce_System.DTOs.ProductDTOs; // DTOs for product-related data transfer.
using E_Commerce_System.DTOs.UserDTOs; // DTOs for user-related data transfer.
using E_Commerce_System.Models; // Domain models like User, Product, etc.
using E_Commerce_System.Repositories; // Interfaces for data access and repository pattern.
using Microsoft.IdentityModel.Tokens; // Token management library for JWT generation.
using System.IdentityModel.Tokens.Jwt; // For working with JWT tokens.
using System.Security.Claims; // For managing claims in JWT tokens.
using System.Text; // For encoding strings.
using System.Text.RegularExpressions; // For validating user input like email and password.

namespace E_Commerce_System.Services
{
    // Service class that provides business logic related to users.
    public class UserService : IUserService
    {
        // Dependencies injected via constructor.
        private readonly IUserRepository _userRepository; // Repository for user-related data operations.
        private readonly IHttpContextAccessor _httpContextAccessor; // Access HTTP context for current user information.
        private readonly IMapper _mapper; // AutoMapper instance to map between domain models and DTOs.

        // Constructor that initializes the dependencies.
        public UserService(IUserRepository userRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Retrieves all users and maps them to UserOutputDTO.
        /// </summary>
        /// <returns>A list of UserOutputDTO.</returns>
        public List<UserOutputDTO> GetAllUsers()
        {
            // Get all users from the repository and order them by user name.
            List<User> users = _userRepository.GetAllUsers()
                .OrderBy(u => u.userName)
                .ToList();

            // If no users are found, throw an exception.
            if (users == null || users.Count == 0)
            {
                throw new InvalidOperationException("No users found");
            }

            // Map the users to UserOutputDTO and return.
            List<UserOutputDTO> userOutputDTOs = _mapper.Map<List<UserOutputDTO>>(users);
            return userOutputDTOs;
        }

        /// <summary>
        /// Retrieves all users with their related data (not in DTO format).
        /// </summary>
        /// <returns>A list of User entities with all related data.</returns>
        public List<User> GetAllUsersWithRelatedData()
        {
            // Get all users from the repository and order by user name.
            List<User> users = _userRepository.GetAllUsers()
                .OrderBy(u => u.userName)
                .ToList();

            // If no users are found, throw an exception.
            if (users == null || users.Count == 0)
            {
                throw new InvalidOperationException("No users found");
            }

            // Return the list of users with all related data.
            return users;
        }

        /// <summary>
        /// Retrieves a specific user by their ID and maps the data to UserOutputDTO.
        /// </summary>
        /// <param name="id">User ID to fetch.</param>
        /// <returns>A UserOutputDTO representing the user.</returns>
        public UserOutputDTO GetUserById(int id)
        {
            // Get the user by ID from the repository.
            User user = _userRepository.GetUserById(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            // Map the user entity to UserOutputDTO and return it.
            UserOutputDTO userOutputDTO = _mapper.Map<UserOutputDTO>(user);
            return userOutputDTO;
        }

        /// <summary>
        /// Retrieves a specific user by their ID with all related data (not in DTO format).
        /// </summary>
        /// <param name="id">User ID to fetch.</param>
        /// <returns>A User entity with all related data.</returns>
        public User GetUserByIdWithRelatedData(int id)
        {
            // Get the user by ID from the repository.
            User user = _userRepository.GetUserById(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            // Return the full user entity with related data.
            return user;
        }

        /// <summary>
        /// Adds a new user after validating the input data and hashing the password.
        /// </summary>
        /// <param name="userInputDTO">UserInputDTO containing the user's data to be added.</param>
        /// <returns>A UserOutputDTO with the added user's details.</returns>
        public UserOutputDTO AddUser(UserInputDTO userInputDTO)
        {
            // Validate that the username is provided.
            if (string.IsNullOrWhiteSpace(userInputDTO.userName))
            {
                throw new ArgumentNullException("User name is required");
            }

            // Validate that the email is provided and follows a valid format.
            if (string.IsNullOrWhiteSpace(userInputDTO.userEmail))
            {
                throw new ArgumentNullException("Email is required");
            }

            // Validate that the password is provided and meets security requirements.
            if (string.IsNullOrWhiteSpace(userInputDTO.userPassword))
            {
                throw new ArgumentNullException("Password is required");
            }

            // Validate password strength using regular expression.
            if (!Regex.IsMatch(userInputDTO.userPassword, @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,}$"))
            {
                throw new InvalidDataException("Password must be at least 8 characters and at least contain 1 letter and 1 number");
            }

            // Validate email format using regular expression.
            if (!Regex.IsMatch(userInputDTO.userEmail, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                throw new InvalidDataException("Email format must be example@example.com");
            }

            // Validate that the phone number is provided and meets length requirements.
            if (string.IsNullOrWhiteSpace(userInputDTO.userPhone))
            {
                throw new ArgumentNullException("User phone is required");
            }
            if (userInputDTO.userPhone.Length < 8 || userInputDTO.userPhone.Length > 20)
            {
                throw new ArgumentOutOfRangeException("Phone number must be between 8 to 20 characters");
            }

            // Validate that the user role is either 'normal user' or 'admin'.
            if (userInputDTO.userRole.ToLower().Trim() != "normal user" && userInputDTO.userRole.ToLower().Trim() != "admin")
            {
                throw new InvalidDataException("User role must be normal user or admin");
            }

            // Check if the email already exists in the system.
            var checkExistingEmail = _userRepository.GetAllUsers()
                .FirstOrDefault(u => u.userEmail == userInputDTO.userEmail);

            if (checkExistingEmail != null)
            {
                throw new InvalidOperationException("User with this email already exists");
            }

            // Hash the password before saving the user.
            string hashedPassword = PasswordHasher.GetHashedPassword(userInputDTO.userPassword);

            // Map the UserInputDTO to the User domain model.
            User user = _mapper.Map<User>(userInputDTO);
            user.userPassword = hashedPassword; // Set the hashed password.

            // Add the user to the repository and map the result to UserOutputDTO.
            UserOutputDTO userOutputDTO = _mapper.Map<UserOutputDTO>(_userRepository.AddUser(user));

            return userOutputDTO;
        }

        /// <summary>
        /// Retrieves users by their role and filters based on the provided role.
        /// </summary>
        /// <param name="role">Role to filter users by.</param>
        /// <returns>A list of UserOutputDTO filtered by role.</returns>
        public List<UserOutputDTO> GetUsersByRole(string role)
        {
            // Get all users and filter by the specified role.
            var usersDtos = GetAllUsers()
                .Where(u => u.userRole == role)
                .ToList();

            // If no users with the specified role are found, throw an exception.
            if (usersDtos.Count == 0)
            {
                throw new InvalidOperationException($"No users with \"{role}\" role found");
            }

            // Return the filtered list of users.
            return usersDtos;
        }

        /// <summary>
        /// Retrieves a user by their login credentials (email and password).
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>A User entity if the credentials are valid.</returns>
        public User GetUserByLoginCredentials(string email, string password)
        {
            // Retrieve the user by email.
            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                throw new KeyNotFoundException("Invalid email");
            }
            else
            {
                // Check if the provided password matches the stored hashed password.
                if (!PasswordHasher.GetPasswordVerification(password, user.userPassword))
                {
                    throw new InvalidDataException("Incorrect Password");
                }
                else
                {
                    // Return the user if the password matches.
                    return user;
                }
            }
        }
    }
}
