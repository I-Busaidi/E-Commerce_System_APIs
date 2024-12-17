using AutoMapper;
using E_Commerce_System.DTOs.UserDTOs;
using E_Commerce_System.Models;
namespace E_Commerce_System
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserInputDTO, User>();
            CreateMap<User, UserOutputDTO>();
        }
    }
}
