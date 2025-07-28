using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SportsCenterApi.Controllers;
using SportsCenterApi.Models;
using SportsCenterApi.Models.DTO;
using SportsCenterApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SportsCenterTest
{
    public class UserControllerTest
    {

        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<UsersController>> _mockLogger;

        private readonly UsersController _userController;

        public UserControllerTest()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<UsersController>>();
            _userController = new UsersController(_mockUserService.Object, _mockLogger.Object);
        }


        [Fact]
        public async Task GetAllUsers_ReturnsOk_WithUsers()
        {
            //Arrange
            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Password = "gdfgdgsfdsgsgrthf",
                    Phone = "12312",
                    RoleId = 1
                },
                new User
                {
                    UserId = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    Password = "rhrgsfdsfsg",
                    Phone = "435453643",
                    RoleId = 2
                }

            };

            _mockUserService.Setup(s => s.GetAllAsync()).ReturnsAsync(users);

            //Act
            var result = await _userController.GetUsers();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserResponseDTO>>(okResult.Value);
            Assert.Equal(2, returnedUsers.Count());
        }

        [Fact]
        public async Task GetUser_ReturnsOk_WhenUserExists()
        {
            //Arrange
            var user = new User
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "hashedpassword",
                Phone = "123456789",
                RoleId = 1
            };
            _mockUserService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(user);

            //Act
            var result = await _userController.GetUser(1);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUser = Assert.IsType<UserResponseDTO>(okResult.Value);
            Assert.Equal(1, returnedUser.UserId);
        }

        [Fact]
        public async Task GetUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            //Arrange
            _mockUserService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((User?)null);

            //Act
            var result = await _userController.GetUser(999);

            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateUser_ReturnsCreated_WhenValidData()
        {
            //Arrange
            var userDto = new UserCreateDto
            {
                FirstName = "New",
                LastName = "User",
                Email = "new.user@example.com",
                Password = "efshfdsdfs",
                Phone = "34234342",
                RoleId = 2
            };

            var createdUser = new User
            {
                UserId = 3,
                FirstName = "New",
                LastName = "User",
                Email = "new.user@example.com",
                Password = "gdrgdfgfd",
                Phone = "352352334",
                RoleId = 2
            };

            _mockUserService.Setup(s => s.RegisterUserAsync(It.IsAny<User>())).ReturnsAsync(createdUser);

            //Act
            var result = await _userController.CreateUser(userDto);

            //Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedUser = Assert.IsType<UserResponseDTO>(createdResult.Value);
            Assert.Equal(3, returnedUser.UserId);
        }
        [Fact]
        public async Task CreateUser_ReturnsConflict_WhenEmailExists()
        {
            //Arrange
            var userDto = new UserCreateDto
            {
                FirstName = "Duplicate",
                LastName = "User",
                Email = "existing@example.com",
                Password = "rfsefdfsdf",
                Phone = "123456789",
                RoleId = 1
            };

            _mockUserService.Setup(s => s.RegisterUserAsync(It.IsAny<User>()))
                           .ThrowsAsync(new InvalidOperationException("Email already exists"));

            //Act
            var result = await _userController.CreateUser(userDto);

            //Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.Equal("Email already exists", conflictResult.Value);
        }

        [Fact]
        public async Task CreateUser_ReturnsBadRequest_WhenInvalidData()
        {
            //Arrange
            var userDto = new UserCreateDto
            {
                FirstName = "",
                LastName = "User",
                Email = "invalid",
                Password = "123",
                Phone = "123456789",
                RoleId = 1
            };

            _mockUserService.Setup(s => s.RegisterUserAsync(It.IsAny<User>()))
                           .ThrowsAsync(new ArgumentException("Invalid user data"));

            //Act
            var result = await _userController.CreateUser(userDto);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid user data", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateUser_ReturnsOk_WhenValidData()
        {
            //Arrange
            var userDto = new UserDTO
            {
                UserId = 1,
                FirstName = "Updated",
                LastName = "User",
                Email = "updated@example.com",
                Phone = "987654321",
                RoleId = 2
            };

            var updatedUser = new User
            {
                UserId = 1,
                FirstName = "Updated",
                LastName = "User",
                Email = "updated@example.com",
                Password = "fefsadafa",
                Phone = "987654321",
                RoleId = 2
            };

            _mockUserService.Setup(s => s.UpdateAsyncUser(1, userDto)).ReturnsAsync(updatedUser);

            //Act
            var result = await _userController.UpdateUser(1, userDto);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUser = Assert.IsType<UserResponseDTO>(okResult.Value);
            Assert.Equal("Updated", returnedUser.FirstName);
        }

        [Fact]
        public async Task UpdateUser_ReturnsBadRequest_WhenIdMismatch()
        {
            //Arrange
            var userDto = new UserDTO { UserId = 2 };

            //Act
            var result = await _userController.UpdateUser(1, userDto);

            //Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task UpdateUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            //Arrange
            var userDto = new UserDTO { UserId = 999 };
            _mockUserService.Setup(s => s.UpdateAsyncUser(999, userDto)).ReturnsAsync((User?)null);

            //Act
            var result = await _userController.UpdateUser(999, userDto);

            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNoContent_WhenUserExists()
        {
            //Arrange
            _mockUserService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            //Act
            var result = await _userController.DeleteUser(1);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            //Arrange
            _mockUserService.Setup(s => s.DeleteAsync(999)).ReturnsAsync(false);

            //Act
            var result = await _userController.DeleteUser(999);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ChangePassword_ReturnsOk_WhenValidPassword()
        {
            //Arrange
            var dto = new ChangePasswordDTO
            {
                CurrentPassword = "oldpassword",
                NewPassword = "newpassword123"
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _userController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = claimsPrincipal }
            };

            _mockUserService.Setup(s => s.ChangePasswordAsync(1, "oldpassword", "newpassword123"))
                           .ReturnsAsync(true);

            //Act
            var result = await _userController.ChangePassword(dto);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Password changed successfully.", okResult.Value);
        }

        [Fact]
        public async Task ChangePassword_ReturnsBadRequest_WhenCurrentPasswordIncorrect()
        {
            //Arrange
            var dto = new ChangePasswordDTO
            {
                CurrentPassword = "wrongpassword",
                NewPassword = "newpassword123"
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _userController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = claimsPrincipal }
            };

            _mockUserService.Setup(s => s.ChangePasswordAsync(1, "wrongpassword", "newpassword123"))
                           .ReturnsAsync(false);

            //Act
            var result = await _userController.ChangePassword(dto);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Current password is incorrect.", badRequestResult.Value);
        }

        [Fact]
        public async Task ChangeOthersPassword_ReturnsOk_WhenUserHasPermission()
        {
            //Arrange
            var dto = new ChangePasswordDTO
            {
                CurrentPassword = "oldpassword",
                NewPassword = "newpassword123"
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("permission", "CHANGE_OTHERS_PASSWORD")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _userController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = claimsPrincipal }
            };

            _mockUserService.Setup(s => s.ChangePasswordAsync(2, "oldpassword", "newpassword123"))
                           .ReturnsAsync(true);

            //Act
            var result = await _userController.ChangeOthersPassword(2, dto);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Password changed successfully.", okResult.Value);
        }

        [Fact]
        public async Task ChangeOthersPassword_ReturnsForbid_WhenUserLacksPermission()
        {
            //Arrange
            var dto = new ChangePasswordDTO
            {
                CurrentPassword = "oldpassword",
                NewPassword = "newpassword123"
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
                
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _userController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = claimsPrincipal }
            };

            //Act
            var result = await _userController.ChangeOthersPassword(2, dto);

            //Assert
            var forbidResult = Assert.IsType<ForbidResult>(result);
        }
    }

}

