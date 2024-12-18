using AutoMapper;
using E_Commerce_System.DTOs.OrderDTOs;
using E_Commerce_System.Models;
using E_Commerce_System.Repositories;

namespace E_Commerce_System.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderProductsService _orderProductsService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IOrderProductsService orderProductsService, IMapper mapper, IUserService userService, IProductService productService, ApplicationDbContext context)
        {
            _orderRepository = orderRepository;
            _orderProductsService = orderProductsService;
            _mapper = mapper;
            _userService = userService;
            _productService = productService;
            _context = context;
        }

        //public (OrderOutputDTO order, List<OrderProductOutputDTO> orderProducts) ConfirmCheckout(int userId)
        //{
        //    User user = _userService.GetUserByIdWithRelatedData(userId);
        //    if (user.userCart.Count == 0)
        //    {
        //        throw new InvalidOperationException("Cart is empty");
        //    }
        //    decimal orderSum = 0;
        //    foreach(var item in user.userCart)
        //    {
        //        if (item.product.productStock < item.quantity)
        //        {
        //            throw new InvalidOperationException($"{item.product.productName} quantity exceeds available stock");
        //        }
        //        else
        //        {
        //            orderSum += item.quantity * item.product.productPrice;
        //        }
        //    }


        //}

        public (OrderOutputDTO order, List<OrderProductOutputDTO> orderProducts) AddOrder(OrderInputDTO orderInputDTO, List<OrderProductInputDTO> orderProductsInputDTO)
        {
            if (orderInputDTO.orderTotalAmount <= 0)
            {
                throw new ArgumentException("invalid order total cost amount");
            }

            if (orderInputDTO.userId == 0)
            {
                throw new ArgumentException("invalid user id");
            }
            Order orderInput = _mapper.Map<Order>(orderInputDTO);
            OrderOutputDTO orderOutput = _mapper.Map<OrderOutputDTO>(_orderRepository.AddOrder(orderInput));
            List<OrderProductOutputDTO> orderProductsOutput = _orderProductsService.AddProducts(orderProductsInputDTO);

            return (orderOutput, orderProductsOutput);
        }

        public List<OrderOutputDTO> GetUserOrders(int id)
        {
            if (id == 0)
            {
                throw new KeyNotFoundException("User not found");
            }
            List<Order> userOrders = _orderRepository.GetOrdersByUserId(id).ToList();
            if (userOrders.Count == 0 || userOrders == null)
            {
                throw new InvalidOperationException("No orders made by this user");
            }

            return _mapper.Map<List<OrderOutputDTO>>(userOrders);
        }

        public (OrderOutputDTO order, List<OrderProductOutputDTO> orderProducts) GetOrderDetails(int id)
        {
            Order order = _orderRepository.GetOrderById(id);
            if (order == null)
            {
                throw new KeyNotFoundException("Order not found");
            }
            if (order.OrderProducts.Count == 0)
            {
                throw new InvalidDataException("Unable to retrieve order details");
            }
            OrderOutputDTO orderOutput = _mapper.Map<OrderOutputDTO>(order);
            List<OrderProductOutputDTO> orderProductsOutput = _mapper.Map<List<OrderProductOutputDTO>>(order.OrderProducts);

            return (orderOutput, orderProductsOutput);
        }
    }
}
