using Azure.Core;
using E_Commerce_System.DTOs.ReviewDTOs;
using E_Commerce_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_System.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[Controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IJwtService _jwtService;

        public ReviewController(IReviewService reviewService, IJwtService jwtService)
        {
            _reviewService = reviewService;
            _jwtService = jwtService;
        }

        [HttpPost("ReviewProduct")]
        public IActionResult ReviewProduct([FromBody] ReviewInputWithProductNameDTO review)
        {
            try
            {
                int userId = _jwtService.DecodeToken(GetToken()).userId;
                int result = _reviewService.AddReview(review, userId);
                return Created(string.Empty, result);
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpGet("GetProductReviews/{productName}")]
        public IActionResult GetAllProductReviews(string productName, int pageNumber = 1, int pageSize = 1)
        {
            try
            {
                var reviews = _reviewService.GetProductReviewsByName(productName)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetReviewsByUser")]
        public IActionResult GetReviewsByUser()
        {
            try
            {
                int userId = _jwtService.DecodeToken(GetToken()).userId;
                var reviews = _reviewService.GetReviewsByUserId(userId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("UpdateReview")]
        public IActionResult UpdateReview([FromBody] ReviewInputWithProductNameDTO review)
        {
            try
            {
                int userId = _jwtService.DecodeToken(GetToken()).userId;
                int result = _reviewService.UpdateReview(review, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpDelete("DeleteReview/{productName}")]
        public IActionResult DeleteProductReview(string productName)
        {
            try
            {
                int userId = _jwtService.DecodeToken(GetToken()).userId;
                int result = _reviewService.DeleteReview(productName, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
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
