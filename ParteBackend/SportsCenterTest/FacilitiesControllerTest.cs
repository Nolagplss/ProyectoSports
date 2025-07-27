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

namespace SportsCenterTest
{
    public class FacilitiesControllerTest
    {
        private readonly Mock<IFacilityService> _mockFacilityService;
        private readonly FacilitiesController _controller;

        public FacilitiesControllerTest()
        {
            _mockFacilityService = new Mock<IFacilityService>();
            _controller = new FacilitiesController(_mockFacilityService.Object);
        }


        [Fact]
        public async Task GetAllFacilities_ReturnsOkResult_WithFacilities()
        {
            //Arrange
            var facilities = new List<Facility>
            {
                new Facility { FacilityId = 1, Name = "Gym", Type = "Gym" },
                new Facility { FacilityId = 2, Name = "Pool", Type = "Pool" }
            };

            _mockFacilityService.Setup(s => s.GetAllAsync())    
                               .ReturnsAsync(facilities);

            //Act
            var result = await _controller.GetAllFacilities();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedFacilities = Assert.IsAssignableFrom<IEnumerable<FacilityDTO>>(okResult.Value);
            Assert.Equal(2, returnedFacilities.Count());
        }

        [Fact]
        public async Task GetFacilityById_ReturnsOk_WhenExists()
        {
            //Arrange
            var facility = new Facility { FacilityId = 1, Name = "Test Gym", Type = "Gym" };
            _mockFacilityService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(facility);

            //Act
            var result = await _controller.GetFacilityById(1);

            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetFacilityById_ReturnsNotFound_WhenNotExists()
        {
            //Arrange
            _mockFacilityService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((Facility?)null);

            //Act
            var result = await _controller.GetFacilityById(999);

            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateFacility_ReturnsCreated_WhenValid()
        {
            //Arrange
            var dto = new FacilityDTO { Name = "New Gym", Type = "Gym" };
            var created = new Facility { FacilityId = 1, Name = "New Gym", Type = "Gym" };
            _mockFacilityService.Setup(s => s.CreateAsync(It.IsAny<Facility>())).ReturnsAsync(created);

            //Act
            var result = await _controller.CreateFacility(dto);

            //Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task CreateFacility_ReturnsBadRequest_WhenInvalidData()
        {
            //Arrange
            var dto = new FacilityDTO { Name = "", Type = "InvalidType" };
            _mockFacilityService.Setup(s => s.CreateAsync(It.IsAny<Facility>()))
                               .ThrowsAsync(new ArgumentException("Invalid data"));

            //Act
            var result = await _controller.CreateFacility(dto);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteFacility_ReturnsNoContent_WhenExists()
        {
            //Arrange
            var facility = new Facility { FacilityId = 1, Name = "Test" };
            _mockFacilityService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(facility);

            //Act
            var result = await _controller.DeleteFacility(1, new FacilityDTO());

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteFacility_ReturnsNotFound_WhenNotExists()
        {
            //Arrange
            _mockFacilityService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((Facility?)null);

            //Act
            var result = await _controller.DeleteFacility(999, new FacilityDTO());

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }

}

