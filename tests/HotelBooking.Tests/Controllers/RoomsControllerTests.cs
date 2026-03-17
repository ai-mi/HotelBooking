using FluentAssertions;
using HotelBooking.EndPoint.Common.Dto;
using HotelBooking.EndPoint.Common.Enums;
using HotelBooking.EndPoint.Controllers;
using HotelBooking.External.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace HotelBooking.Tests.Controllers;

public class RoomsControllerTests
{
    private readonly Mock<IRoomService> _mockRoomService;
    private readonly Mock<ILogger<RoomsController>> _mockLogger;
    private readonly RoomsController _controller;

    public RoomsControllerTests()
    {
        _mockRoomService = new Mock<IRoomService>();
        _mockLogger = new Mock<ILogger<RoomsController>>();
        _controller = new RoomsController(_mockRoomService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetRoomsByCategory_ShouldReturnOkWithRooms()
    {
        // Arrange
        var category = RoomCategory.Standard;
        var rooms = new List<RoomDto>
        {
            new RoomDto
            {
                Id = Guid.NewGuid(),
                RoomNumber = "101",
                Category = RoomCategory.Standard,
                PricePerNight = 100,
                MaxOccupancy = 1
            }
        };

        _mockRoomService.Setup(s => s.GetRoomsByCategoryAsync(category))
            .ReturnsAsync(rooms);

        // Act
        var result = await _controller.GetRoomsByCategory(category);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedRooms = okResult.Value.Should().BeAssignableTo<IEnumerable<RoomDto>>().Subject;
        returnedRooms.Should().HaveCount(1);
        returnedRooms.First().Category.Should().Be(RoomCategory.Standard);
    }

    [Fact]
    public async Task GetRoomsByCategory_WhenExceptionThrown_ShouldReturn500()
    {
        // Arrange
        var category = RoomCategory.Standard;
        _mockRoomService.Setup(s => s.GetRoomsByCategoryAsync(category))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetRoomsByCategory(category);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task SearchAvailableRooms_WithValidData_ShouldReturnOkWithRooms()
    {
        // Arrange
        var searchDto = new RoomAvailabilitySearchDto
        {
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(3)),
            NumberOfGuests = 2
        };

        var availableRooms = new List<AvailableRoomDto>
        {
            new AvailableRoomDto
            {
                RoomId = Guid.NewGuid(),
                RoomNumber = "101",
                Category = RoomCategory.Family,
                PricePerNight = 150,
                MaxOccupancy = 2,
                TotalPrice = 300
            }
        };

        _mockRoomService.Setup(s => s.SearchAvailableRoomsAsync(searchDto))
            .ReturnsAsync(availableRooms);

        // Act
        var result = await _controller.SearchAvailableRooms(searchDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedRooms = okResult.Value.Should().BeAssignableTo<IEnumerable<AvailableRoomDto>>().Subject;
        returnedRooms.Should().HaveCount(1);
    }


    [Fact]
    public async Task SearchAvailableRooms_WithCheckOutBeforeCheckIn_ShouldReturnBadRequest()
    {
        // Arrange
        var searchDto = new RoomAvailabilitySearchDto
        {
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(3)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)), // Before check-in
            NumberOfGuests = 2
        };

        // Act
        var result = await _controller.SearchAvailableRooms(searchDto);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.StatusCode.Should().Be(400);
    }


    [Fact]
    public async Task SearchAvailableRooms_WhenExceptionThrown_ShouldReturn500()
    {
        // Arrange
        var searchDto = new RoomAvailabilitySearchDto
        {
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(3)),
            NumberOfGuests = 2
        };

        _mockRoomService.Setup(s => s.SearchAvailableRoomsAsync(searchDto))
            .ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _controller.SearchAvailableRooms(searchDto);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task SearchAvailableRooms_WithNoAvailableRooms_ShouldReturnEmptyList()
    {
        // Arrange
        var searchDto = new RoomAvailabilitySearchDto
        {
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(3)),
            NumberOfGuests = 2
        };

        _mockRoomService.Setup(s => s.SearchAvailableRoomsAsync(searchDto))
            .ReturnsAsync(new List<AvailableRoomDto>());

        // Act
        var result = await _controller.SearchAvailableRooms(searchDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedRooms = okResult.Value.Should().BeAssignableTo<IEnumerable<AvailableRoomDto>>().Subject;
        returnedRooms.Should().BeEmpty();
    }
}
