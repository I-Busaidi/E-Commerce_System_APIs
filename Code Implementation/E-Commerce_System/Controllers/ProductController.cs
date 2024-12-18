using Azure.Core;
using E_Commerce_System.DTOs.ProductDTOs;
using E_Commerce_System.DTOs.UserDTOs;
using E_Commerce_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_System.Controllers
{
    [Authorize(Policy = "HasAdminRole")]
    [ApiController]
    [Route("api/[Controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IJwtService _jwtService;

        public ProductController(IProductService productService, IJwtService jwtService)
        {
            _productService = productService;
            _jwtService = jwtService;
        }

        [HttpPost("Add-Product")]
        public IActionResult AddProduct([FromBody] ProductInputDTO productInputDTO)
        {
            try
            {
                var product = _productService.AddProduct(productInputDTO);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpPut("Update-Product/{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] ProductInputDTO productInputDTO)
        {
            try
            {
                var product = _productService.UpdateProduct(productInputDTO, id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("GetAllProducts")]
        public IActionResult GetAllProducts(
            int pageNumber = 1,
            int pageSize = 1,
            string searchName = "",
            decimal minPrice = 0,
            decimal maxPrice = int.MaxValue)
        {
            try
            {
                var products = _productService.GetAllProducts()
                    .Where(p => p.productName.ToLower().Trim().Contains(searchName.ToLower().Trim())
                    & p.productPrice >= minPrice & p.productPrice <= maxPrice)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();


                return Ok(products);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("GetProductById/{id}")]
        public IActionResult GetProductById(int id)
        {
            try
            {
                var product = _productService.GetProductById(id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [NonAction]
        public string GetToken()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Token unavailable or expired");
            }

            return token;
        }
    }
}
