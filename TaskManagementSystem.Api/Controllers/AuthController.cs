using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Api.Services;
using TaskManagementSystem.Core.DTOs;
using TaskManagementSystem.Core.Services;

namespace TaskManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtService _jwtService;

        public AuthController(IUserService userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            // Add debugging
            Console.WriteLine($"Login attempt for user: {request.Username}");
            
            var user = await _userService.AuthenticateAsync(request.Username, request.Password);

            if (user == null)
            {
                Console.WriteLine("Authentication failed");
                return Unauthorized(new { message = "Invalid username or password" });
            }

            Console.WriteLine($"Authentication successful for user: {user.Username}");
            var token = _jwtService.GenerateToken(user);

            return Ok(new LoginResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Role = user.Role,
                Token = token
            });
        }
    }
}