using Microsoft.AspNetCore.Mvc;
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
    public class ReservationControllerTest
    {

        private readonly Mock<IReservationService> _mockReservationService;
        private readonly ReservationController _reservationController;

        public ReservationControllerTest()  
        {
            _mockReservationService = new Mock<IReservationService>();
            _reservationController = new ReservationController(_mockReservationService.Object);
        }

        [Fact]
        public async Task GetAllReservations_ReturnsOk_WithReservations()
        {
            //Arrange
            var reservations = new List<Reservation>
            {
                new Reservation
                {
                    ReservationId = 1,
                    UserId = 1,
                    FacilityId = 1,
                    ReservationDate = new DateOnly(2024, 12, 15),
                    StartTime = new TimeOnly(10, 0),
                    EndTime = new TimeOnly(11, 0),
                    PaymentCompleted = true,
                    NoShow = false
                },
                new Reservation
                {
                    ReservationId = 2,
                    UserId = 2,
                    FacilityId = 2,
                    ReservationDate = new DateOnly(2024, 12, 16),
                    StartTime = new TimeOnly(14, 30),
                    EndTime = new TimeOnly(15, 30),
                    PaymentCompleted = false,
                    NoShow = null
                }
            };


            _mockReservationService.Setup(s => s.GetAllAsync()).ReturnsAsync(reservations);


            //Act
            var result = await _reservationController.GetAllReservationsAsync();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedReservations = Assert.IsAssignableFrom<IEnumerable<ReservationResponseDTO>>(okResult.Value);
            Assert.Equal(2, returnedReservations.Count());


        }

        [Fact]
        public async Task FilterReservations_WithValidDateRange_ReturnsOk()
        {
            //Arrange
            var startDate = new DateOnly(2024, 12, 1);
            var endDate = new DateOnly(2024, 12, 31);
            var filteredReservations = new List<Reservation>
            {
                new Reservation
                {
                    ReservationId = 1,
                    UserId = 1,
                    FacilityId = 1,
                    ReservationDate = new DateOnly(2024, 12, 15),
                    StartTime = new TimeOnly(10, 0),
                    EndTime = new TimeOnly(11, 0),
                    PaymentCompleted = true,
                    NoShow = false
                }
            };

            _mockReservationService.Setup(s => s.FilterReservationsAsync(
                It.IsAny<int?>(), It.IsAny<string?>(), It.IsAny<string?>(), startDate, endDate))
                .ReturnsAsync(filteredReservations);

            // Act
            var result = await _reservationController.FilterReservations(null, null, null, startDate, endDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedReservations = Assert.IsType<List<Reservation>>(okResult.Value);
            Assert.Single(returnedReservations);
        }

        [Fact]
        public async Task FilterReservations_WithInvalidDateRange_ReturnsBadRequest()
        {
            //Arrange
            var startDate = new DateOnly(2024, 12, 31);
            var endDate = new DateOnly(2024, 12, 1); 

            //Act
            var result = await _reservationController.FilterReservations(null, null, null, startDate, endDate);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Start date cannot be after end date.", badRequestResult.Value);
        }

        //Theory with multiple validations to wirte less code
        [Theory]
        [InlineData(1, "gym", null, 1)]           //Only userId and facilityType
        [InlineData(null, "pool", "Main Pool", 2)] //facilityType and facilityName
        [InlineData(2, null, "Tennis Court", 1)]   //userId and facilityName
        [InlineData(null, null, null, 3)]          //Witouth filters, all reservations
        public async Task FilterReservations_VariousCombinations_ReturnsExpectedCount(
           int? userId, string? facilityType, string? facilityName, int expectedCount)
        {
            //Arrange
            var expectedReservations = CreateMockReservations(expectedCount);

            _mockReservationService.Setup(s => s.FilterReservationsAsync(
                userId, facilityType, facilityName, null, null))
                .ReturnsAsync(expectedReservations);

            //Act
            var result = await _reservationController.FilterReservations(userId, facilityType, facilityName, null, null);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedReservations = Assert.IsType<List<Reservation>>(okResult.Value);
            Assert.Equal(expectedCount, returnedReservations.Count);
        }

        private List<Reservation> CreateMockReservations(int count)
        {
            var reservations = new List<Reservation>();
            for (int i = 1; i <= count; i++)
            {
                reservations.Add(new Reservation
                {
                    ReservationId = i,
                    UserId = i,
                    FacilityId = i,
                    ReservationDate = new DateOnly(2024, 12, 10 + i),
                    StartTime = new TimeOnly(10 + i, 0),
                    EndTime = new TimeOnly(11 + i, 0),
                    PaymentCompleted = true,
                    NoShow = false
                });
            }
            return reservations;
        }


    }
}
