using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Moq;
using SportsCenterApi.Controllers;
using SportsCenterApi.Models;
using SportsCenterApi.Models.DTO;
using SportsCenterApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsCenterTest
{
    public class RoleControllerTest
    {

        private readonly Mock<IRoleService> _mockRoleService;
        private readonly RoleController _roleController;

        public RoleControllerTest()
        {
            _mockRoleService = new Mock<IRoleService>();
            _roleController = new RoleController(_mockRoleService.Object);
        }


        [Fact]
        public async Task GetAllRoles_returnsOk_WithRoles()
        {

            //Arrange
            var roles = new List<Role>
            {
                new Role
                {
                    RoleId = 1,
                    RoleName = "Admin",
                    Permissions = new List<Permission>
                    {
                        new Permission { PermissionId = 1, Description = "Full access", Code = "ADMIN_ALL" }
                    }
                },
                new Role
                {
                    RoleId = 2,
                    RoleName = "Member",
                    Permissions = new List<Permission>
                    {
                        new Permission { PermissionId = 2, Description = "Basic access", Code = "MEMBER_BASIC" }
                    }
                }
            };

            _mockRoleService.Setup(s => s.GetAllAsync()).ReturnsAsync(roles);

            //Act
            var result = await _roleController.GetAllRoles();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRoles = Assert.IsAssignableFrom<IEnumerable<RoleDTO>>(okResult.Value);
            Assert.Equal(2, returnedRoles.Count());



        }

        [Fact]
        public async Task GetRoleById_ReturnsOk_WithRole()
        {
            //Arrange
            var roleId = 1;
            var role = new Role
            {
                RoleId = roleId,
                RoleName = "Administrator",
                Permissions = new List<Permission>
                {
                    new Permission { PermissionId = 1, Description = "Full access", Code = "ADMIN_ALL" }
                }
            };

            _mockRoleService.Setup(s => s.GetByIdAsync(roleId)).ReturnsAsync(role);

            //Act
            var result = await _roleController.GetRoleById(roleId);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRole = Assert.IsType<RoleDTO>(okResult.Value);
            Assert.Equal(roleId, returnedRole.RoleId);
            Assert.Equal("Administrator", returnedRole.RoleName);
            Assert.Single(returnedRole.Permissions);
        }

        [Fact]
        public async Task CreateRole_ReturnsCreatedAtAction_WithValidData()
        {
            //Arrange
            var roleDto = new RoleDTO
            {
                RoleName = "NewRole",
                Permissions = new List<PermissionsDTO>()
            };
            var createdRole = new Role
            {
                RoleId = 1,
                RoleName = "NewRole",
                Permissions = new List<Permission>()
            };

            _mockRoleService.Setup(s => s.CreateAsync(It.IsAny<Role>())).ReturnsAsync(createdRole);

            //Act
            var result = await _roleController.CreateRole(roleDto);

            //Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(_roleController.GetRoleById), createdResult.ActionName);
            Assert.Equal(1, createdResult.RouteValues["id"]);

            var returnedDto = Assert.IsType<RoleDTO>(createdResult.Value);
            Assert.Equal("NewRole", returnedDto.RoleName);
        }

        [Fact]
        public async Task CreateRole_ReturnsConflict_WhenRoleAlreadyExists()
        {
            //Arrange
            var roleDto = new RoleDTO
            {
                RoleName = "ExistingRole",
                Permissions = new List<PermissionsDTO>()
            };

            _mockRoleService.Setup(s => s.CreateAsync(It.IsAny<Role>()))
                .ThrowsAsync(new InvalidOperationException("Role already exists"));

            //Act
            var result = await _roleController.CreateRole(roleDto);

            //Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.Equal("Role already exists", conflictResult.Value);
        }

        [Fact]
        public async Task UpdateRole_ReturnsOk_WithUpdatedRole()
        {
            //Arrange
            var roleId = 1;
            var roleDto = new RoleDTO
            {
                RoleId = roleId,
                RoleName = "UpdatedRole",
                Permissions = new List<PermissionsDTO>()
            };
            var updatedRole = new Role
            {
                RoleId = roleId,
                RoleName = "UpdatedRole",
                Permissions = new List<Permission>()
            };

            _mockRoleService.Setup(s => s.UpdateAsyncRole(roleId, roleDto)).ReturnsAsync(updatedRole);

            //Act
            var result = await _roleController.UpdateRole(roleId, roleDto);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<RoleDTO>(okResult.Value);
            Assert.Equal("UpdatedRole", returnedDto.RoleName);
        }

        [Fact]
        public async Task DeleteRole_ReturnsNoContent_WhenRoleExists()
        {
            //Arrange
            var roleId = 999;
            var roleDto = new RoleDTO();

            _mockRoleService.Setup(s => s.GetByIdAsync(roleId)).ReturnsAsync((Role)null);

            //Act
            var result = await _roleController.DeleteRole(roleId, roleDto);

            //Assert
            Assert.IsType<NotFoundResult>(result);
            _mockRoleService.Verify(s => s.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteRole_ReturnsNotFound_WhenRoleDoesNotExist()
        {
            //Arrange
            var roleId = 999;
            var roleDto = new RoleDTO();

            _mockRoleService.Setup(s => s.GetByIdAsync(roleId)).ReturnsAsync((Role)null);

            //Act
            var result = await _roleController.DeleteRole(roleId, roleDto);

            //Assert
            Assert.IsType<NotFoundResult>(result);
            _mockRoleService.Verify(s => s.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
