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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IOrderProductsService orderProductsService, IMapper mapper, IUserService userService, IProductService productService, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _orderRepository = orderRepository;
            _orderProductsService = orderProductsService;
            _mapper = mapper;
            _userService = userService;
            _productService = productService;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public (string productName, int quantity, decimal productSum) AddItemToCart(int userId, string productName, int quantity)
        {
            if (quantity < 1)
            {
                throw new ArgumentOutOfRangeException("quantity must be 1 or above");
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

            var cart = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<List<(Product product, int quantity)>>("Cart") ?? new List<(Product, int)>();

            var existingItem = cart.FirstOrDefault(c => c.product.productName == productName );
            if (existingItem.product != null)
            {
                cart.Remove(existingItem);
                existingItem.quantity += quantity;
                cart.Add(existingItem);
            }
            else
            {
                cart.Add((product, quantity));
            }

            _httpContextAccessor.HttpContext.Session.SetObjectAsJson("Cart", cart);

            return (product.productName, quantity, product.productPrice * quantity);
        }

        public string RemoveItemFromCart(int userId, string productName)
        {
            var cart = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<List<(Product product, int quantity)>>("Cart") ?? new List<(Product, int)>();

            var existingItem = cart.FirstOrDefault(c => c.product.productName == productName);
            if (existingItem.product != null)
            {
                cart.Remove(existingItem);
            }
            else
            {
                throw new InvalidOperationException("Product not found");
            }

            _httpContextAccessor.HttpContext.Session.SetObjectAsJson("Cart", cart);

            return productName;
        }

        public (OrderOutputDTO order, List<OrderProductOutputDTO> orderProducts) ConfirmCheckout(int userId)
        {
            var cart = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<List<(Product product, int quantity)>>("Cart");
            if (cart == null || cart.Count == 0)
            {
                throw new InvalidOperationException("Cart is empty");
            }
            List<OrderProductOutputDTO> orderProducts = new List<OrderProductOutputDTO>();
            decimal orderSum = 0;
            foreach (var item in cart)
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

                    foreach (var item in cart)
                    {
                        _productService.UpdateProductStock(item.product, item.quantity * -1);
                        orderProducts.Add(_orderProductsService.AddProducts(new OrderProducts
                        {
                            Order = order,
                            orderId = order.orderId,
                            Product = item.product,
                            productId = item.product.productId,
                            productQuantity = item.quantity
                        }));
                    }

                    _context.SaveChanges();
                    transaction.Commit();

                    _httpContextAccessor.HttpContext.Session.Remove("Cart");

                    return (_mapper.Map<OrderOutputDTO>(order), orderProducts);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new InvalidOperationException("An error occured while checking out "+ ex.Message.ToString());
                }
            }
        }

        public Order AddOrder(OrderInputDTO orderInputDTO)
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
            orderInput.userId = orderInputDTO.userId;
            return _orderRepository.AddOrder(orderInput);
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

            List<OrderProductOutputDTO> orderProductsOutput = _orderProductsService.GetOrderProducts(order.orderId);
            OrderOutputDTO orderOutput = _mapper.Map<OrderOutputDTO>(order);

            return (orderOutput, orderProductsOutput);
        }
    }
}
