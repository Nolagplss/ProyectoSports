using Microsoft.AspNetCore.Mvc;
using Moq;
using SportsCenterApi.Controllers;
using SportsCenterApi.Models.DTO;
using SportsCenterApi.Models;
using SportsCenterApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace SportsCenterTest
{
    public class AuthControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ItokenService> _mockTokenService;
        private readonly AuthController _controller;
        private readonly ITestOutputHelper _output;  


        public AuthControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _mockUserService = new Mock<IUserService>();
            _mockTokenService = new Mock<ItokenService>();
            _controller = new AuthController(_mockUserService.Object, _mockTokenService.Object);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenUserIsNull()
        {
            //Arrange
            var dto = new LoginDto { Email = "test@example.com", Password = "wrongpassword" };

            _mockUserService.Setup(s => s.AuthenticateAsync(dto.Email, dto.Password)).ReturnsAsync((User?)null);

            //Act
            var result = await _controller.Login(dto);

            //Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsOk_WithToken_WhenCredentialsAreValid()
        {
            // Arrange
            var dto = new LoginDto { Email = "test@example.com", Password = "correctpassword" };

            var mockUser = new User { UserId = 1, Email = dto.Email };

            _mockUserService.Setup(s => s.AuthenticateAsync(dto.Email, dto.Password))
                .ReturnsAsync(mockUser);

            _mockTokenService.Setup(s => s.CreateToken(mockUser))
                .Returns("fake-jwt-token");

            //Act
            var result = await _controller.Login(dto);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);


            _output.WriteLine($"Returned Value Type: {okResult.Value.GetType().FullName}");
        }


    }
}
