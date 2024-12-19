using AutoMapper;
using E_Commerce_System.DTOs.OrderDTOs;
using E_Commerce_System.DTOs.ReviewDTOs;
using E_Commerce_System.Models;
using E_Commerce_System.Services;

namespace E_Commerce_System.CustomValueResolvers
{
    public class ProductNameResolver : IValueResolver<OrderProducts, OrderProductOutputDTO, string>
    {
        private readonly IProductService _productService;

        public ProductNameResolver(IProductService productService)
        {
            _productService = productService;
        }

        public string Resolve(OrderProducts source, OrderProductOutputDTO destination, string member, ResolutionContext context)
        {
            var product = _productService.GetProductById(source.productId);
            return product?.productName ?? "Unknown";
        }
    }

    public class ProductPriceResolver : IValueResolver<OrderProducts, OrderProductOutputDTO, decimal>
    {
        private readonly IProductService _productService;
        public ProductPriceResolver(IProductService productService)
        {
            _productService = productService;
        }
        public decimal Resolve(OrderProducts source, OrderProductOutputDTO destination, decimal member, ResolutionContext context)
        {
            var product = _productService.GetProductById(source.productId);
            return product?.productPrice ?? 0m;
        }
    }

    public class ReviewProductNameResolver : IValueResolver<Review, ReviewOutputDTO, string>
    {
        private readonly IProductService _productService;

        public ReviewProductNameResolver(IProductService productService)
        {
            _productService = productService;
        }

        public string Resolve(Review source, ReviewOutputDTO destination, string member, ResolutionContext context)
        {
            var product = _productService.GetProductById(source.productId);
            return product?.productName ?? "Unknown";
        }
    }
}
