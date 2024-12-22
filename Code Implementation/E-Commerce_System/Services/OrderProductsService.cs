using AutoMapper; // AutoMapper is used for mapping between different object models (e.g., entity to DTO).
using E_Commerce_System.DTOs.OrderDTOs; // Contains DTOs for Order-related data transfer objects.
using E_Commerce_System.Models; // Contains the models representing the database entities (e.g., OrderProducts).
using E_Commerce_System.Repositories; // Contains repository interfaces for data access.

namespace E_Commerce_System.Services
{
    // Service class for handling business logic related to order products.
    // It implements IOrderProductsService to provide the necessary methods.
    public class OrderProductsService : IOrderProductsService
    {
        // Private readonly fields for repository and AutoMapper instance
        private readonly IOrderProductsRepository _orderProductsRepository; // Repository interface for interacting with order products data.
        private readonly IMapper _mapper; // AutoMapper instance for mapping between entity models and DTOs.

        // Constructor that takes in dependencies for repository and mapper.
        // These dependencies are injected via dependency injection (DI).
        public OrderProductsService(IOrderProductsRepository orderProductsRepository, IMapper mapper)
        {
            _orderProductsRepository = orderProductsRepository; // Assign the repository instance.
            _mapper = mapper; // Assign the AutoMapper instance.
        }

        /// <summary>
        /// Adds products to an order and maps the result to the corresponding output DTO.
        /// </summary>
        /// <param name="orderProductsInput">The order products data to be added.</param>
        /// <returns>An OrderProductOutputDTO representing the added order product.</returns>
        public OrderProductOutputDTO AddProducts(OrderProducts orderProductsInput)
        {
            // Check if the input is null. If so, throw an exception indicating invalid input.
            if (orderProductsInput == null)
            {
                throw new InvalidOperationException("Order products not found"); // Throw an exception for invalid input.
            }

            // Add the order products using the repository and map the result to an OrderProductOutputDTO.
            // The result of AddOrderProduct is mapped using AutoMapper to match the OrderProductOutputDTO.
            return _mapper.Map<OrderProductOutputDTO>(_orderProductsRepository.AddOrderProduct(orderProductsInput));
        }

        /// <summary>
        /// Retrieves all products associated with a specific order.
        /// </summary>
        /// <param name="id">The ID of the order for which products need to be fetched.</param>
        /// <returns>A list of OrderProductOutputDTOs representing the order products.</returns>
        public List<OrderProductOutputDTO> GetOrderProducts(int id)
        {
            // Fetch order products from the repository based on the given order ID.
            var orderProducts = _orderProductsRepository.GetOrdersProductsById(id).ToList();

            // If no order products are found, throw an exception indicating no results.
            if (orderProducts == null)
            {
                throw new InvalidOperationException("No order products found"); // Throw an exception if no products are found.
            }

            // Map the retrieved order products to a list of OrderProductOutputDTOs.
            List<OrderProductOutputDTO> orderProductsOutput = _mapper.Map<List<OrderProductOutputDTO>>(orderProducts);

            // Return the mapped list of OrderProductOutputDTOs.
            return orderProductsOutput;
        }
    }
}
