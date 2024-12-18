using AutoMapper;
using E_Commerce_System.DTOs.OrderDTOs;
using E_Commerce_System.DTOs.ProductDTOs;
using E_Commerce_System.DTOs.UserDTOs;
using E_Commerce_System.Models;
using E_Commerce_System.Services;
namespace E_Commerce_System
{
    public class MappingProfile : Profile
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        public MappingProfile()
        {
            CreateMap<UserInputDTO, User>();
            CreateMap<User, UserOutputDTO>();

            CreateMap<ProductInputDTO, Product>();
            CreateMap<Product, ProductOutputDTO>();


            CreateMap<OrderInputDTO, Order>();

            CreateMap<Order, OrderOutputDTO>()
                .ForMember(dest => dest.userName, opt => opt.MapFrom((src, dest) => GetUserNameFromId(src.userId)));


            CreateMap<OrderProductInputDTO, OrderProducts>();

            CreateMap<OrderProducts, OrderProductOutputDTO>()
                .ForMember(dest => dest.productName, opt => opt.MapFrom((src, dest) => GetProductInfoFromId(src.productId).name))
                .ForMember(dest => dest.productPrice, opt => opt.MapFrom((src, dest) => GetProductInfoFromId(src.productId).price));
        }

        private string GetUserNameFromId(int  id)
        {
            var user = _userService.GetUserById(id);
            return user.userName;
        }

        private (string name, decimal price) GetProductInfoFromId(int id)
        {
            var product = _productService.GetProductById(id);
            return (product.productName, product.productPrice);
        }
    }
}
