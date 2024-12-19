using AutoMapper;
using E_Commerce_System.DTOs.OrderDTOs;
using E_Commerce_System.DTOs.ReviewDTOs;
using E_Commerce_System.Models;
using E_Commerce_System.Services;

namespace E_Commerce_System.CustomValueResolvers
{
    public class UserNameResolver : IValueResolver<Order, OrderOutputDTO, string>
    {
        private readonly IUserService _userService;

        public UserNameResolver(IUserService userService)
        {
            _userService = userService;
        }

        public string Resolve(Order source, OrderOutputDTO destination, string member, ResolutionContext context)
        {
            var user = _userService.GetUserById(source.userId);
            return user?.userName ?? "Unknown";
        }
    }

    public class ReviewUserNameResolver : IValueResolver<Review, ReviewOutputDTO, string>
    {
        private readonly IUserService _userService;

        public ReviewUserNameResolver(IUserService userService)
        {
            _userService = userService;
        }

        public string Resolve(Review source, ReviewOutputDTO destination, string member, ResolutionContext context)
        {
            var user = _userService.GetUserById(source.userId);
            return user?.userName ?? "Unknown";
        }
    }
}
