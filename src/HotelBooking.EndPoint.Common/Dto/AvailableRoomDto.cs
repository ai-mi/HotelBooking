using HotelBooking.EndPoint.Common.Enums;

namespace HotelBooking.EndPoint.Common.Dto;

public class AvailableRoomDto
{
    public Guid RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public RoomCategory Category { get; set; }
    public decimal PricePerNight { get; set; }
    public int MaxOccupancy { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
}
