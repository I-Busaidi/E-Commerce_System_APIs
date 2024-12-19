using E_Commerce_System.DTOs.UserDTOs;
using E_Commerce_System.Models;
using E_Commerce_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_Commerce_System.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[Controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public UserController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [AllowAnonymous]
        [HttpPost("Add-User")]
        public IActionResult AddUser([FromBody] UserInputDTO userInputDTO)
        {
            try
            {
                var user = _userService.AddUser(userInputDTO);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("Login")]
        public IActionResult UserLogin(string email, string password)
        {
            try
            {
                var user = _userService.GetUserByLoginCredentials(email, password);
                if (user == null)
                {
                    return Unauthorized("Invalid Credentials");
                }
                else
                {
                    string token = _jwtService.GenerateJwtToken(user.userId.ToString(), user.userName, user.userRole);

                    return Ok(token);
                }
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
        }

        [HttpGet("GetUserInfo")]
        public IActionResult GetUserInfo()
        {
            try
            {
                int userId = _jwtService.DecodeToken(GetToken()).userId;
                var user = _userService.GetUserById(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("ViewCart")]
        public IActionResult ViewCart()
        {
            try
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<(Product product, int quantity)>>("Cart") ?? new List<(Product, int)>();
                return Ok(cart);
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
