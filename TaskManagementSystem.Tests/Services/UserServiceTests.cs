using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Services;
using Xunit;

namespace TaskManagementSystem.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IPasswordHashService> _mockPasswordHashService;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockPasswordHashService = new Mock<IPasswordHashService>();
            _userService = new UserService(_mockUserRepository.Object, _mockPasswordHashService.Object);
        }

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var username = "testuser";
            var password = "testpassword";
            var hashedPassword = "hashedpassword";

            var user = new User
            {
                Id = 1,
                Username = username,
                Password = hashedPassword,
                Email = "test@example.com",
                Role = "User"
            };

            _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(username))
                .ReturnsAsync(user);

            _mockPasswordHashService.Setup(service => service.VerifyPassword(hashedPassword, password))
                .Returns(true);

            // Act
            var result = await _userService.AuthenticateAsync(username, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Username, result.Username);

            // Verify the repository and password service were called
            _mockUserRepository.Verify(repo => repo.GetByUsernameAsync(username), Times.Once);
            _mockPasswordHashService.Verify(service => service.VerifyPassword(hashedPassword, password), Times.Once);
        }

        [Fact]
        public async Task AuthenticateAsync_InvalidUsername_ReturnsNull()
        {
            // Arrange
            var username = "nonexistentuser";
            var password = "testpassword";

            _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(username))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.AuthenticateAsync(username, password);

            // Assert
            Assert.Null(result);

            // Verify the repository was called, but password service was not
            _mockUserRepository.Verify(repo => repo.GetByUsernameAsync(username), Times.Once);
            _mockPasswordHashService.Verify(service => service.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task AuthenticateAsync_InvalidPassword_ReturnsNull()
        {
            // Arrange
            var username = "testuser";
            var password = "wrongpassword";
            var hashedPassword = "hashedpassword";

            var user = new User
            {
                Id = 1,
                Username = username,
                Password = hashedPassword,
                Email = "test@example.com",
                Role = "User"
            };

            _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(username))
                .ReturnsAsync(user);

            _mockPasswordHashService.Setup(service => service.VerifyPassword(hashedPassword, password))
                .Returns(false);

            // Act
            var result = await _userService.AuthenticateAsync(username, password);

            // Assert
            Assert.Null(result);

            // Verify both the repository and password service were called
            _mockUserRepository.Verify(repo => repo.GetByUsernameAsync(username), Times.Once);
            _mockPasswordHashService.Verify(service => service.VerifyPassword(hashedPassword, password), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_HashesPasswordBeforeSaving()
        {
            // Arrange
            var plainPassword = "plainPassword";
            var hashedPassword = "hashedPassword";

            var user = new User
            {
                Username = "newuser",
                Password = plainPassword,
                Email = "newuser@example.com",
                Role = "User"
            };

            var createdUser = new User
            {
                Id = 1,
                Username = "newuser",
                Password = hashedPassword,
                Email = "newuser@example.com",
                Role = "User"
            };

            _mockPasswordHashService.Setup(service => service.HashPassword(plainPassword))
                .Returns(hashedPassword);

            _mockUserRepository.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(createdUser);

            // Act
            var result = await _userService.CreateUserAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("newuser", result.Username);

            // Verify the password was hashed before saving
            _mockPasswordHashService.Verify(service => service.HashPassword(plainPassword), Times.Once);
            
            // Verify the repository was called with the user that has the hashed password
            _mockUserRepository.Verify(repo => repo.AddAsync(It.Is<User>(u => u.Password == hashedPassword)), Times.Once);
        }
    }
}
