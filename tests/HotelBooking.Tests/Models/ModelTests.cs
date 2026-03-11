using FluentAssertions;
using HotelBooking.EndPoint.Common.Enums;
using HotelBooking.EndPoint.Common.Models;

namespace HotelBooking.Tests.Models;

public class RoomTests
{
    [Fact]
    public void Room_ShouldInitializeWithDefaultValues()
    {
        // Act
        var room = new Room();

        // Assert
        room.RoomNumber.Should().Be(string.Empty);
        room.Description.Should().Be(string.Empty);
        room.Bookings.Should().NotBeNull();
        room.Bookings.Should().BeEmpty();
        room.AuditLogs.Should().NotBeNull();
        room.AuditLogs.Should().BeEmpty();
    }

    [Fact]
    public void Room_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var hotelId = Guid.NewGuid();

        // Act
        var room = new Room
        {
            Id = roomId,
            RoomNumber = "101",
            HotelId = hotelId,
            Category = RoomCategory.Standard,
            PricePerNight = 100.00m,
            Floor = 1,
            MaxOccupancy = 1,
            Status = RoomStatus.Available,
            Description = "Comfortable single room"
        };

        // Assert
        room.Id.Should().Be(roomId);
        room.RoomNumber.Should().Be("101");
        room.HotelId.Should().Be(hotelId);
        room.Category.Should().Be(RoomCategory.Standard);
        room.PricePerNight.Should().Be(100.00m);
        room.Floor.Should().Be(1);
        room.MaxOccupancy.Should().Be(1);
        room.Status.Should().Be(RoomStatus.Available);
        room.Description.Should().Be("Comfortable single room");
    }

    [Theory]
    [InlineData(RoomCategory.Standard)]
    [InlineData(RoomCategory.Deluxe)]
    [InlineData(RoomCategory.Suite)]
    [InlineData(RoomCategory.Penthouse)]
    [InlineData(RoomCategory.Family)]
    [InlineData(RoomCategory.Executive)]
    public void Room_ShouldAcceptAllValidCategories(RoomCategory category)
    {
        // Arrange & Act
        var room = new Room { Category = category };

        // Assert
        room.Category.Should().Be(category);
    }

}
