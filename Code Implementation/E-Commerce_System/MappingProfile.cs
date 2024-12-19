using AutoMapper;
using E_Commerce_System.CustomValueResolvers;
using E_Commerce_System.DTOs.OrderDTOs;
using E_Commerce_System.DTOs.ProductDTOs;
using E_Commerce_System.DTOs.UserDTOs;
using E_Commerce_System.Models;
using E_Commerce_System.Services;
namespace E_Commerce_System
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserInputDTO, User>();
            CreateMap<User, UserOutputDTO>();

            CreateMap<ProductInputDTO, Product>();
            CreateMap<Product, ProductOutputDTO>();


            CreateMap<OrderInputDTO, Order>();
            CreateMap<Order, OrderOutputDTO>()
                .ForMember(dest => dest.userName, opt => opt.MapFrom<UserNameResolver>());


            CreateMap<OrderProductInputDTO, OrderProducts>();

            CreateMap<OrderProducts, OrderProductOutputDTO>()
                .ForMember(dest => dest.productName, opt => opt.MapFrom<ProductNameResolver>())
                .ForMember(dest => dest.productPrice, opt => opt.MapFrom<ProductPriceResolver>());
        }

        private string GetUserNameFromId(int id, IUserService userService)
        {
            var user = userService.GetUserById(id);
            return user?.userName ?? "Unknown";
        }

        private (string name, decimal price) GetProductInfoFromId(int id, IProductService productService)
        {
            var product = productService.GetProductById(id);
            return (product.productName, product.productPrice);
        }
    }
}
