﻿using E_Commerce_System.DTOs.ProductDTOs;
using E_Commerce_System.Models;

namespace E_Commerce_System.Services
{
    public interface IProductService
    {
        ProductOutputDTO AddProduct(ProductInputDTO productInputDTO);
        List<ProductOutputDTO> GetAllProducts();
        List<Product> GetAllProductsWithRelatedData();
        ProductOutputDTO GetProductById(int id);
        Product GetProductByIdWithRelatedData(int id);
        ProductOutputDTO UpdateProduct(ProductInputDTO productInputDTO, int id);
    }
}