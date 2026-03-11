using HotelBooking.EndPoint.Common.Enums;

namespace HotelBooking.EndPoint.Common.Dto;

public class RoomDto
{
    public Guid Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public Guid HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public RoomCategory Category { get; set; }
    public decimal PricePerNight { get; set; }
    public int Floor { get; set; }
    public int MaxOccupancy { get; set; }
    public RoomStatus Status { get; set; }
    public string Description { get; set; } = string.Empty;
}
