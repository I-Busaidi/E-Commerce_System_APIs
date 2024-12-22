using AutoMapper; // AutoMapper is used to map between domain models and DTOs.
using E_Commerce_System.DTOs.ProductDTOs; // DTOs for product-related data transfer.
using E_Commerce_System.DTOs.UserDTOs; // DTOs for user-related data (although not used here).
using E_Commerce_System.Models; // Domain models like Product, Order, etc.
using E_Commerce_System.Repositories; // Interfaces for data access and repository pattern.

namespace E_Commerce_System.Services
{
    // Service class responsible for handling business logic related to products.
    // Implements IProductService interface to expose product-related functionalities.
    public class ProductService : IProductService
    {
        // Dependencies injected via constructor
        private readonly IProductRepository _productRepository; // Repository for product-related data operations.
        private readonly IMapper _mapper; // AutoMapper instance to convert between entities and DTOs.

        // Constructor to initialize service dependencies.
        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all products from the repository and maps them to ProductOutputDTO.
        /// </summary>
        /// <returns>A list of ProductOutputDTO representing all products.</returns>
        public List<ProductOutputDTO> GetAllProducts()
        {
            // Get all products with related data.
            List<Product> products = GetAllProductsWithRelatedData();

            // Map the product list to ProductOutputDTO.
            List<ProductOutputDTO> productOutputDTOs = _mapper.Map<List<ProductOutputDTO>>(products);
            return productOutputDTOs;
        }

        /// <summary>
        /// Retrieves all products from the repository with full related data.
        /// </summary>
        /// <returns>A list of Product entities.</returns>
        public List<Product> GetAllProductsWithRelatedData()
        {
            // Fetch all products from the repository, ordered by product name.
            List<Product> products = _productRepository.GetAllProducts()
                .OrderBy(p => p.productName)
                .ToList();

            // If no products are found, throw an exception.
            if (products == null || products.Count == 0)
            {
                throw new InvalidOperationException("No products found");
            }

            return products;
        }

        /// <summary>
        /// Retrieves a product by its name from the repository.
        /// </summary>
        /// <param name="productName">The name of the product to fetch.</param>
        /// <returns>The Product entity.</returns>
        public Product GetProductByName(string productName)
        {
            // Fetch the product from the repository by product name.
            var product = _productRepository.GetAllProducts()
                .FirstOrDefault(p => p.productName == productName);

            // If the product doesn't exist, throw a KeyNotFoundException.
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            return product;
        }

        /// <summary>
        /// Retrieves a product by its ID and maps it to ProductOutputDTO.
        /// </summary>
        /// <param name="id">The ID of the product to fetch.</param>
        /// <returns>The ProductOutputDTO representing the product.</returns>
        public ProductOutputDTO GetProductById(int id)
        {
            // Fetch the product with related data by ID.
            Product product = GetProductByIdWithRelatedData(id);

            // Map the product entity to a ProductOutputDTO.
            ProductOutputDTO productOutputDTO = _mapper.Map<ProductOutputDTO>(product);
            return productOutputDTO;
        }

        /// <summary>
        /// Retrieves a product by its ID from the repository with full related data.
        /// </summary>
        /// <param name="id">The ID of the product to fetch.</param>
        /// <returns>The Product entity.</returns>
        public Product GetProductByIdWithRelatedData(int id)
        {
            // Fetch the product by its ID from the repository.
            Product product = _productRepository.GetProductById(id);

            // If the product is not found, throw a KeyNotFoundException.
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            return product;
        }

        /// <summary>
        /// Adds a new product based on the provided ProductInputDTO.
        /// </summary>
        /// <param name="productInputDTO">The DTO containing the product data to add.</param>
        /// <returns>The ProductOutputDTO representing the added product.</returns>
        public ProductOutputDTO AddProduct(ProductInputDTO productInputDTO)
        {
            // Validate the input data for null or invalid values.
            if (productInputDTO == null)
            {
                throw new ArgumentNullException("Unable to process entry");
            }

            if (string.IsNullOrWhiteSpace(productInputDTO.productName))
            {
                throw new ArgumentException("Product name is required");
            }

            if (productInputDTO.productPrice <= 0)
            {
                throw new ArgumentOutOfRangeException("Price must be greater than 0");
            }

            if (productInputDTO.productStock < 0)
            {
                throw new ArgumentOutOfRangeException("Stock cannot be less than 0");
            }

            // Map the DTO to the Product entity.
            Product product = _mapper.Map<Product>(productInputDTO);

            // Add the product to the repository and map the result to ProductOutputDTO.
            ProductOutputDTO productOutputDTO = _mapper.Map<ProductOutputDTO>(_productRepository.AddProduct(product));

            return productOutputDTO;
        }

        /// <summary>
        /// Updates the stock of a product by adding or subtracting the specified amount.
        /// </summary>
        /// <param name="product">The product whose stock is to be updated.</param>
        /// <param name="amount">The amount to update the stock by (can be positive or negative).</param>
        public void UpdateProductStock(Product product, int amount)
        {
            // Update the product's stock by the specified amount.
            product.productStock += amount;

            // Save the updated stock to the repository.
            _productRepository.StockUpdateProduct(product);
        }

        /// <summary>
        /// Updates the product's rating in the repository.
        /// </summary>
        /// <param name="product">The product whose rating is to be updated.</param>
        /// <param name="avgRating">The new average rating to set.</param>
        public void UpdateProductRating(Product product, decimal? avgRating)
        {
            // Update the product's rating with the new average rating.
            product.productRating = avgRating;

            // Save the updated rating to the repository.
            _productRepository.RatingUpdateProduct(product);
        }

        /// <summary>
        /// Updates an existing product using the provided ProductInputDTO.
        /// </summary>
        /// <param name="productInputDTO">The DTO containing the updated product data.</param>
        /// <param name="id">The ID of the product to update.</param>
        /// <returns>The updated ProductOutputDTO representing the product.</returns>
        public ProductOutputDTO UpdateProduct(ProductInputDTO productInputDTO, int id)
        {
            // Validate the input data for null or invalid values.
            if (productInputDTO == null)
            {
                throw new ArgumentNullException("Unable to process entry");
            }

            if (string.IsNullOrWhiteSpace(productInputDTO.productName))
            {
                throw new ArgumentException("Product name is required");
            }

            if (productInputDTO.productPrice <= 0)
            {
                throw new ArgumentOutOfRangeException("Price must be greater than 0");
            }

            if (productInputDTO.productStock < 0)
            {
                throw new ArgumentOutOfRangeException("Stock cannot be less than 0");
            }

            // Fetch the existing product by ID.
            var existingProduct = _productRepository.GetProductById(id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            // Check if the product name already exists for another product.
            bool nameExists = _productRepository.GetAllProducts()
                .Any(p => p.productName == productInputDTO.productName && p.productId != id);
            if (nameExists)
            {
                throw new ArgumentException("A product with this name already exists");
            }

            // Update the product properties with the new data.
            existingProduct.productPrice = productInputDTO.productPrice;
            existingProduct.productDescription = productInputDTO.productDescription;
            existingProduct.productName = productInputDTO.productName;
            existingProduct.productStock = productInputDTO.productStock;

            // Map the updated product to ProductOutputDTO.
            ProductOutputDTO productOutputDTO = _mapper.Map<ProductOutputDTO>(_productRepository.UpdateProduct(existingProduct));

            return productOutputDTO;
        }
    }
}
