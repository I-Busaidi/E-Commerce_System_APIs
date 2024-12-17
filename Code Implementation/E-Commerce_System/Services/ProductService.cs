using AutoMapper;
using E_Commerce_System.DTOs.ProductDTOs;
using E_Commerce_System.DTOs.UserDTOs;
using E_Commerce_System.Models;
using E_Commerce_System.Repositories;

namespace E_Commerce_System.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public List<ProductOutputDTO> GetAllProducts()
        {
            List<Product> products = GetAllProductsWithRelatedData();
            List<ProductOutputDTO> productOutputDTOs = _mapper.Map<List<ProductOutputDTO>>(products);
            return productOutputDTOs;
        }

        public List<Product> GetAllProductsWithRelatedData()
        {
            List<Product> products = _productRepository.GetAllProducts()
                .OrderBy(p => p.productName)
                .ToList();
            if (products == null || products.Count == 0)
            {
                throw new InvalidOperationException("No products found");
            }

            return products;
        }

        public ProductOutputDTO GetProductById(int id)
        {
            Product product = GetProductByIdWithRelatedData(id);
            ProductOutputDTO productOutputDTO = _mapper.Map<ProductOutputDTO>(product);
            return productOutputDTO;
        }

        public Product GetProductByIdWithRelatedData(int id)
        {
            Product product = _productRepository.GetProductById(id);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            return product;
        }

        public ProductOutputDTO AddProduct(ProductInputDTO productInputDTO)
        {
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

            Product product = _mapper.Map<Product>(productInputDTO);

            ProductOutputDTO productOutputDTO = _mapper.Map<ProductOutputDTO>(_productRepository.AddProduct(product));

            return productOutputDTO;
        }

        public ProductOutputDTO UpdateProduct(ProductInputDTO productInputDTO, int id)
        {
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

            var existingProduct = _productRepository.GetProductById(id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            bool nameExists = _productRepository.GetAllProducts()
                .Any(p => p.productName == productInputDTO.productName & p.productId != id);
            if (nameExists)
            {
                throw new ArgumentException("A product with this name already exists");
            }

            existingProduct.productPrice = productInputDTO.productPrice;
            existingProduct.productDescription = productInputDTO.productDescription;
            existingProduct.productName = productInputDTO.productName;
            existingProduct.productStock = productInputDTO.productStock;

            ProductOutputDTO productOutputDTO = _mapper.Map<ProductOutputDTO>(_productRepository.UpdateProduct(existingProduct));

            return productOutputDTO;
        }
    }
}
