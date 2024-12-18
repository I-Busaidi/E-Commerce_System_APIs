using AutoMapper;
using E_Commerce_System.DTOs.OrderDTOs;
using E_Commerce_System.Models;
using E_Commerce_System.Repositories;

namespace E_Commerce_System.Services
{
    public class OrderService : IOrderService
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

        public (string productName, int quantity, decimal productSum) AddItemToCart(int userId, string productName, int quantity)
        {
            var user = _userService.GetUserByIdWithRelatedData(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            var product = _productService.GetProductByName(productName);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }
            if (product.productStock < quantity)
            {
                throw new InvalidOperationException("Product stock insufficient");
            }

            user.userCart.Add((product, quantity));
            return (product.productName, quantity, product.productPrice * quantity);
        }

        public (OrderOutputDTO order, List<OrderProductOutputDTO> orderProducts) ConfirmCheckout(int userId)
        {
            User user = _userService.GetUserByIdWithRelatedData(userId);
            if (user.userCart.Count == 0)
            {
                throw new InvalidOperationException("Cart is empty");
            }
            List<OrderProductInputDTO> orderProducts = new List<OrderProductInputDTO>();
            decimal orderSum = 0;
            foreach (var item in user.userCart)
            {
                if (item.product.productStock < item.quantity)
                {
                    throw new InvalidOperationException($"{item.product.productName} quantity exceeds available stock");
                }
                else
                {
                    orderSum += (item.quantity * item.product.productPrice);
                }
            }

            // transaction
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var order = AddOrder(new OrderInputDTO
                    {
                        orderTotalAmount = orderSum,
                        userId = userId,
                    });

                    foreach (var item in user.userCart)
                    {
                        _productService.UpdateProductStock(item.product, item.quantity * -1);
                        orderProducts.Add(new OrderProductInputDTO
                        {
                            orderId = order.orderId,
                            productId = item.product.productId,
                            productQuantity = item.quantity
                        });
                    }

                    var orderedProducts = _orderProductsService.AddProducts(orderProducts);

                    _context.SaveChanges();
                    transaction.Commit();

                    user.userCart.Clear();

                    return (order, orderedProducts);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new InvalidOperationException("An error occured while checking out "+ ex.Message.ToString());
                }
            }
        }

        public OrderOutputDTO AddOrder(OrderInputDTO orderInputDTO)
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

            return orderOutput;
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
        public List<Order> GetUserOrdersWithRelatedData(int id)
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

            return userOrders;
        }

        public (OrderOutputDTO order, List<OrderProductOutputDTO> orderProducts) GetOrderDetails(int orderId, int userId)
        {
            Order? order = GetUserOrdersWithRelatedData(userId).FirstOrDefault(o => o.orderId == orderId);
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
