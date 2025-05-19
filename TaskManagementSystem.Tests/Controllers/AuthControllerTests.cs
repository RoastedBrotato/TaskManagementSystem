using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using TaskManagementSystem.Api.Controllers;
using TaskManagementSystem.Api.Services;
using TaskManagementSystem.Core.DTOs;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Services;
using Xunit;

namespace TaskManagementSystem.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly JwtService _jwtService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockUserService = new Mock<IUserService>();

            // Create a configuration with the JWT secret
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Jwt:Secret"]).Returns("test-secret-key-for-unit-tests-with-sufficient-length");

            // Create an actual instance of JwtService with the mocked configuration
            _jwtService = new JwtService(configurationMock.Object);

            _controller = new AuthController(_mockUserService.Object, _jwtService);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "testpassword"
            };

            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                Role = "User"
            };

            _mockUserService.Setup(service => service.AuthenticateAsync(loginRequest.Username, loginRequest.Password))
                .ReturnsAsync(user);

            // Act
            var actionResult = await _controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var response = Assert.IsType<LoginResponse>(okResult.Value);

            Assert.Equal(user.Id, response.UserId);
            Assert.Equal(user.Username, response.Username);
            Assert.Equal(user.Role, response.Role);
            Assert.NotNull(response.Token); // We can't predict the exact token, but it should not be null

            // Verify service calls
            _mockUserService.Verify(service => service.AuthenticateAsync(loginRequest.Username, loginRequest.Password), Times.Once);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "wrongpassword"
            };

            _mockUserService.Setup(service => service.AuthenticateAsync(loginRequest.Username, loginRequest.Password))
                .ReturnsAsync((User)null);

            // Act
            var actionResult = await _controller.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);

            // Extract and verify the message
            var responseValue = unauthorizedResult.Value;
            Assert.NotNull(responseValue);

            // Use reflection to access the 'message' property of the anonymous object
            var messageProperty = responseValue.GetType().GetProperty("message");
            Assert.NotNull(messageProperty);
            var message = messageProperty.GetValue(responseValue) as string;
            Assert.Equal("Invalid username or password", message);

            // Verify service call
            _mockUserService.Verify(service => service.AuthenticateAsync(loginRequest.Username, loginRequest.Password), Times.Once);
        }
    }
}