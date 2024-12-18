using AutoMapper;
using E_Commerce_System.DTOs.OrderDTOs;
using E_Commerce_System.Models;
using E_Commerce_System.Repositories;

namespace E_Commerce_System.Services
{
    public class OrderProductsService : IOrderProductsService
    {
        private readonly IOrderProductsRepository _orderProductsRepository;
        private readonly IMapper _mapper;

        public OrderProductsService(IOrderProductsRepository orderProductsRepository, IMapper mapper)
        {
            _orderProductsRepository = orderProductsRepository;
            _mapper = mapper;
        }

        public List<OrderProductOutputDTO> AddProducts(List<OrderProductInputDTO> orderProductsInputDTO)
        {
            List<OrderProducts> orderProductsInput = _mapper.Map<List<OrderProducts>>(orderProductsInputDTO);
            if (orderProductsInput == null || orderProductsInput.Count == 0)
            {
                throw new InvalidOperationException("Order products not found");
            }

            return _mapper.Map<List<OrderProductOutputDTO>>(_orderProductsRepository.AddOrderProduct(orderProductsInput));
        }
    }
}
