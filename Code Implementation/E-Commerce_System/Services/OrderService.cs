using AutoMapper; // AutoMapper is used to map between domain models and DTOs.
using E_Commerce_System.DTOs.OrderDTOs; // DTOs related to orders.
using E_Commerce_System.Models; // Domain models like Order, Product, etc.
using E_Commerce_System.Repositories; // Interfaces for data access and repository pattern.

namespace E_Commerce_System.Services
{
    // Service class for handling the business logic related to orders.
    // Implements IOrderService interface to provide methods for order management.
    public class OrderService : IOrderService
    {
        // Dependencies injected via constructor
        private readonly ApplicationDbContext _context; // Context for database access.
        private readonly IOrderRepository _orderRepository; // Repository for handling Order entity operations.
        private readonly IOrderProductsService _orderProductsService; // Service for handling operations related to products in orders.
        private readonly IUserService _userService; // Service for user-related operations.
        private readonly IProductService _productService; // Service for product-related operations.
        private readonly IHttpContextAccessor _httpContextAccessor; // Accessor for HTTP context (e.g., for session management).
        private readonly IMapper _mapper; // AutoMapper instance to map between entities and DTOs.

        // Constructor that initializes the service with required dependencies.
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

        /// <summary>
        /// Adds an item to the user's shopping cart.
        /// </summary>
        /// <param name="userId">The ID of the user adding the item.</param>
        /// <param name="productName">The name of the product to add.</param>
        /// <param name="quantity">The quantity of the product to add.</param>
        /// <returns>Returns a tuple with the product name, quantity, and product total price.</returns>
        public (string productName, int quantity, decimal productSum) AddItemToCart(int userId, string productName, int quantity)
        {
            // Check if the quantity is valid (must be 1 or more).
            if (quantity < 1)
            {
                throw new ArgumentOutOfRangeException("quantity must be 1 or above");
            }

            // Fetch product by name using the ProductService.
            var product = _productService.GetProductByName(productName);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            // Check if there is enough stock available for the requested quantity.
            if (product.productStock < quantity)
            {
                throw new InvalidOperationException("Product stock insufficient");
            }

            // Retrieve the current cart from session or initialize a new one if empty.
            var cart = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<List<(Product product, int quantity)>>("Cart") ?? new List<(Product, int)>();

            // Check if the product already exists in the cart.
            var existingItem = cart.FirstOrDefault(c => c.product.productName == productName);
            if (existingItem.product != null)
            {
                // If product exists in the cart, remove the old entry and update the quantity.
                cart.Remove(existingItem);
                existingItem.quantity += quantity;
                cart.Add(existingItem);
            }
            else
            {
                // If product is not in the cart, add it as a new entry.
                cart.Add((product, quantity));
            }

            // Save the updated cart back to session.
            _httpContextAccessor.HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Return the product details and the total price for this item.
            return (product.productName, quantity, product.productPrice * quantity);
        }

        /// <summary>
        /// Removes an item from the user's shopping cart.
        /// </summary>
        /// <param name="userId">The ID of the user removing the item.</param>
        /// <param name="productName">The name of the product to remove.</param>
        /// <returns>Returns the name of the removed product.</returns>
        public string RemoveItemFromCart(int userId, string productName)
        {
            // Retrieve the current cart from session.
            var cart = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<List<(Product product, int quantity)>>("Cart") ?? new List<(Product, int)>();

            // Find the item in the cart by product name.
            var existingItem = cart.FirstOrDefault(c => c.product.productName == productName);
            if (existingItem.product != null)
            {
                // If product exists, remove it from the cart.
                cart.Remove(existingItem);
            }
            else
            {
                // If product not found in the cart, throw an exception.
                throw new InvalidOperationException("Product not found");
            }

            // Save the updated cart back to session.
            _httpContextAccessor.HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Return the name of the removed product.
            return productName;
        }

        /// <summary>
        /// Confirms the checkout process by validating the cart and creating an order.
        /// </summary>
        /// <param name="userId">The ID of the user confirming the checkout.</param>
        /// <returns>Returns the order details along with the order product details.</returns>
        public (OrderOutputDTO order, List<OrderProductOutputDTO> orderProducts) ConfirmCheckout(int userId)
        {
            // Retrieve the current cart from session.
            var cart = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<List<(Product product, int quantity)>>("Cart");
            if (cart == null || cart.Count == 0)
            {
                throw new InvalidOperationException("Cart is empty");
            }

            // List to store the order product details.
            List<OrderProductOutputDTO> orderProducts = new List<OrderProductOutputDTO>();
            decimal orderSum = 0;

            // Validate product stock and calculate the total order sum.
            foreach (var item in cart)
            {
                if (item.product.productStock < item.quantity)
                {
                    throw new InvalidOperationException($"{item.product.productName} quantity exceeds available stock");
                }
                else
                {
                    orderSum += (item.quantity * item.product.productPrice); // Add to order total.
                }
            }

            // Start a transaction for the checkout process.
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Create the order in the database.
                    var order = AddOrder(new OrderInputDTO
                    {
                        orderTotalAmount = orderSum,
                        userId = userId,
                    });

                    // Process each product in the cart and update stock.
                    foreach (var item in cart)
                    {
                        _productService.UpdateProductStock(item.product, item.quantity * -1); // Decrease stock by quantity.
                        orderProducts.Add(_orderProductsService.AddProducts(new OrderProducts
                        {
                            Order = order,
                            orderId = order.orderId,
                            Product = item.product,
                            productId = item.product.productId,
                            productQuantity = item.quantity
                        }));
                    }

                    // Commit the transaction and clear the cart session.
                    _context.SaveChanges();
                    transaction.Commit();
                    _httpContextAccessor.HttpContext.Session.Remove("Cart");

                    // Return the created order and the associated order products.
                    return (_mapper.Map<OrderOutputDTO>(order), orderProducts);
                }
                catch (Exception ex)
                {
                    // Rollback transaction if an error occurs during the process.
                    transaction.Rollback();
                    throw new InvalidOperationException("An error occurred while checking out: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Adds a new order to the system.
        /// </summary>
        /// <param name="orderInputDTO">The input data transfer object containing order details.</param>
        /// <returns>The created order entity.</returns>
        public Order AddOrder(OrderInputDTO orderInputDTO)
        {
            // Validate the order total amount and user ID.
            if (orderInputDTO.orderTotalAmount <= 0)
            {
                throw new ArgumentException("Invalid order total cost amount");
            }

            if (orderInputDTO.userId == 0)
            {
                throw new ArgumentException("Invalid user id");
            }

            // Map the DTO to the Order entity.
            Order orderInput = _mapper.Map<Order>(orderInputDTO);
            orderInput.userId = orderInputDTO.userId;

            // Add the order to the repository.
            return _orderRepository.AddOrder(orderInput);
        }

        /// <summary>
        /// Retrieves a list of orders placed by a specific user.
        /// </summary>
        /// <param name="id">The user ID to fetch orders for.</param>
        /// <returns>A list of orders made by the user.</returns>
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

        /// <summary>
        /// Retrieves orders along with their related data for a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>A list of orders with related data.</returns>
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

        /// <summary>
        /// Retrieves details of a specific order and its associated products.
        /// </summary>
        /// <param name="orderId">The ID of the order to retrieve.</param>
        /// <param name="userId">The ID of the user to fetch order for.</param>
        /// <returns>A tuple with the order details and the associated order products.</returns>
        public (OrderOutputDTO order, List<OrderProductOutputDTO> orderProducts) GetOrderDetails(int orderId, int userId)
        {
            Order? order = GetUserOrdersWithRelatedData(userId).FirstOrDefault(o => o.orderId == orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("Order not found");
            }

            List<OrderProductOutputDTO> orderProductsOutput = _orderProductsService.GetOrderProducts(order.orderId);
            OrderOutputDTO orderOutput = _mapper.Map<OrderOutputDTO>(order);

            // Return the order details and its associated products.
            return (orderOutput, orderProductsOutput);
        }
    }
}
