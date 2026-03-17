using FluentAssertions;
using HotelBooking.EndPoint.Common.Dto;
using HotelBooking.EndPoint.Common.Enums;
using HotelBooking.EndPoint.Common.Interfaces;
using HotelBooking.EndPoint.Common.Models;
using HotelBooking.External.Services;
using Moq;

namespace HotelBooking.Tests.Services;

public class RoomServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRepository<Room>> _mockRoomRepository;
    private readonly Mock<IRepository<Hotel>> _mockHotelRepository;
    private readonly Mock<IRepository<Booking>> _mockBookingRepository;
    private readonly RoomService _roomService;

    public RoomServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockRoomRepository = new Mock<IRepository<Room>>();
        _mockHotelRepository = new Mock<IRepository<Hotel>>();
        _mockBookingRepository = new Mock<IRepository<Booking>>();

        _mockUnitOfWork.Setup(uow => uow.Rooms).Returns(_mockRoomRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.Hotels).Returns(_mockHotelRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.Bookings).Returns(_mockBookingRepository.Object);

        _roomService = new RoomService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetAllRoomsAsync_ShouldReturnAllRooms()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var hotel = new Hotel
        {
            Id = hotelId,
            Name = "Test Hotel",
            Address = "123 Test St",
            City = "Test City",
            Country = "Test Country"
        };

        var rooms = new List<Room>
        {
            new Room
            {
                Id = Guid.NewGuid(),
                RoomNumber = "101",
                HotelId = hotelId,
                Hotel = hotel,
                Category = RoomCategory.Standard,
                PricePerNight = 100,
                Floor = 1,
                MaxOccupancy = 1,
                Status = RoomStatus.Available,
                Description = "Single room"
            },
            new Room
            {
                Id = Guid.NewGuid(),
                RoomNumber = "102",
                HotelId = hotelId,
                Hotel = hotel,
                Category = RoomCategory.Deluxe,
                PricePerNight = 150,
                Floor = 1,
                MaxOccupancy = 2,
                Status = RoomStatus.Available,
                Description = "Double room"
            }
        };

        _mockRoomRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(rooms);
        _mockHotelRepository.Setup(h => h.GetByIdAsync(hotelId)).ReturnsAsync(hotel);

        // Act
        var result = await _roomService.GetAllRoomsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(r => r.RoomNumber == "101");
        result.Should().Contain(r => r.RoomNumber == "102");
    }

    [Fact]
    public async Task GetAllRoomsAsync_WhenNoRooms_ShouldReturnEmptyList()
    {
        // Arrange
        _mockRoomRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Room>());

        // Act
        var result = await _roomService.GetAllRoomsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }


    [Fact]
    public async Task SearchAvailableRoomsAsync_ShouldFilterByHotelId()
    {
        // Arrange
        var hotelId1 = Guid.NewGuid();
        var hotelId2 = Guid.NewGuid();
        var hotel1 = new Hotel { Id = hotelId1, Name = "Hotel 1", Address = "Address 1", City = "City 1", Country = "Country 1" };
        var hotel2 = new Hotel { Id = hotelId2, Name = "Hotel 2", Address = "Address 2", City = "City 2", Country = "Country 2" };

        var rooms = new List<Room>
        {
            new Room
            {
                Id = Guid.NewGuid(),
                RoomNumber = "101",
                HotelId = hotelId1,
                Hotel = hotel1,
                Category = RoomCategory.Standard,
                PricePerNight = 100,
                MaxOccupancy = 2,
                Status = RoomStatus.Available,
                Description = "Room in Hotel 1"
            },
            new Room
            {
                Id = Guid.NewGuid(),
                RoomNumber = "201",
                HotelId = hotelId2,
                Hotel = hotel2,
                Category = RoomCategory.Deluxe,
                PricePerNight = 150,
                MaxOccupancy = 2,
                Status = RoomStatus.Available,
                Description = "Room in Hotel 2"
            }
        };

        _mockRoomRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(rooms);
        _mockHotelRepository.Setup(h => h.GetByIdAsync(hotelId1)).ReturnsAsync(hotel1);
        _mockHotelRepository.Setup(h => h.GetByIdAsync(hotelId2)).ReturnsAsync(hotel2);
        _mockBookingRepository.Setup(b => b.GetAllAsync()).ReturnsAsync(new List<Booking>());

        var searchDto = new RoomAvailabilitySearchDto
        {
            HotelId = hotelId1,
            CheckInDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
            NumberOfGuests = 2
        };

        // Act
        var result = await _roomService.SearchAvailableRoomsAsync(searchDto);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().RoomNumber.Should().Be("101");
    }

    [Fact]
    public async Task SearchAvailableRoomsAsync_ShouldFilterByCategory()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var hotel = new Hotel { Id = hotelId, Name = "Test Hotel", Address = "Address", City = "City", Country = "Country" };

        var rooms = new List<Room>
        {
            new Room
            {
                Id = Guid.NewGuid(),
                RoomNumber = "101",
                HotelId = hotelId,
                Hotel = hotel,
                Category = RoomCategory.Standard,
                PricePerNight = 100,
                MaxOccupancy = 2,
                Status = RoomStatus.Available,
                Description = "Single room"
            },
            new Room
            {
                Id = Guid.NewGuid(),
                RoomNumber = "201",
                HotelId = hotelId,
                Hotel = hotel,
                Category = RoomCategory.Suite,
                PricePerNight = 300,
                MaxOccupancy = 4,
                Status = RoomStatus.Available,
                Description = "Suite"
            }
        };

        _mockRoomRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(rooms);
        _mockHotelRepository.Setup(h => h.GetByIdAsync(hotelId)).ReturnsAsync(hotel);
        _mockBookingRepository.Setup(b => b.GetAllAsync()).ReturnsAsync(new List<Booking>());

        var searchDto = new RoomAvailabilitySearchDto
        {
            Category = RoomCategory.Suite,
            CheckInDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
            NumberOfGuests = 2
        };

        // Act
        var result = await _roomService.SearchAvailableRoomsAsync(searchDto);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Category.Should().Be(RoomCategory.Suite);
    }

    [Fact]
    public async Task SearchAvailableRoomsAsync_ShouldFilterByMaxOccupancy()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var hotel = new Hotel { Id = hotelId, Name = "Test Hotel", Address = "Address", City = "City", Country = "Country" };

        var rooms = new List<Room>
        {
            new Room
            {
                Id = Guid.NewGuid(),
                RoomNumber = "101",
                HotelId = hotelId,
                Hotel = hotel,
                Category = RoomCategory.Standard,
                PricePerNight = 100,
                MaxOccupancy = 1,
                Status = RoomStatus.Available,
                Description = "Single room"
            },
            new Room
            {
                Id = Guid.NewGuid(),
                RoomNumber = "201",
                HotelId = hotelId,
                Hotel = hotel,
                Category = RoomCategory.Suite,
                PricePerNight = 300,
                MaxOccupancy = 4,
                Status = RoomStatus.Available,
                Description = "Suite"
            }
        };

        _mockRoomRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(rooms);
        _mockHotelRepository.Setup(h => h.GetByIdAsync(hotelId)).ReturnsAsync(hotel);
        _mockBookingRepository.Setup(b => b.GetAllAsync()).ReturnsAsync(new List<Booking>());

        var searchDto = new RoomAvailabilitySearchDto
        {
            CheckInDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
            NumberOfGuests = 3
        };

        // Act
        var result = await _roomService.SearchAvailableRoomsAsync(searchDto);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().MaxOccupancy.Should().BeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task SearchAvailableRoomsAsync_ShouldCalculateTotalPrice()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var hotel = new Hotel { Id = hotelId, Name = "Test Hotel", Address = "Address", City = "City", Country = "Country" };

        var room = new Room
        {
            Id = Guid.NewGuid(),
            RoomNumber = "101",
            HotelId = hotelId,
            Hotel = hotel,
            Category = RoomCategory.Standard,
            PricePerNight = 100,
            MaxOccupancy = 2,
            Status = RoomStatus.Available,
            Description = "Test room"
        };

        _mockRoomRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Room> { room });
        _mockHotelRepository.Setup(h => h.GetByIdAsync(hotelId)).ReturnsAsync(hotel);
        _mockBookingRepository.Setup(b => b.GetAllAsync()).ReturnsAsync(new List<Booking>());

        var searchDto = new RoomAvailabilitySearchDto
        {
            CheckInDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.Today.AddDays(4)), // 3 nights
            NumberOfGuests = 2
        };

        // Act
        var result = await _roomService.SearchAvailableRoomsAsync(searchDto);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().TotalPrice.Should().Be(300); // 100 * 3 nights
    }

    [Fact]
    public async Task SearchAvailableRoomsAsync_ShouldExcludeUnavailableRooms()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var hotel = new Hotel { Id = hotelId, Name = "Test Hotel", Address = "Address", City = "City", Country = "Country" };

        var rooms = new List<Room>
        {
            new Room
            {
                Id = Guid.NewGuid(),
                RoomNumber = "101",
                HotelId = hotelId,
                Hotel = hotel,
                Category = RoomCategory.Standard,
                PricePerNight = 100,
                MaxOccupancy = 2,
                Status = RoomStatus.Available,
                Description = "Available room"
            },
            new Room
            {
                Id = Guid.NewGuid(),
                RoomNumber = "102",
                HotelId = hotelId,
                Hotel = hotel,
                Category = RoomCategory.Standard,
                PricePerNight = 100,
                MaxOccupancy = 2,
                Status = RoomStatus.Occupied,
                Description = "Occupied room"
            },
            new Room
            {
                Id = Guid.NewGuid(),
                RoomNumber = "103",
                HotelId = hotelId,
                Hotel = hotel,
                Category = RoomCategory.Standard,
                PricePerNight = 100,
                MaxOccupancy = 2,
                Status = RoomStatus.Maintenance,
                Description = "Maintenance room"
            }
        };

        _mockRoomRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(rooms);
        _mockHotelRepository.Setup(h => h.GetByIdAsync(hotelId)).ReturnsAsync(hotel);
        _mockBookingRepository.Setup(b => b.GetAllAsync()).ReturnsAsync(new List<Booking>());

        var searchDto = new RoomAvailabilitySearchDto
        {
            CheckInDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
            NumberOfGuests = 2
        };

        // Act
        var result = await _roomService.SearchAvailableRoomsAsync(searchDto);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().RoomNumber.Should().Be("101");
    }
}
