using HotelBooking.EndPoint.Common.Enums;

namespace HotelBooking.EndPoint.Common.Dto;

public class CreateRoomDto
{
    public string RoomNumber { get; set; } = string.Empty;
    public Guid HotelId { get; set; }
    public RoomCategory Category { get; set; }
    public decimal PricePerNight { get; set; }
    public int Floor { get; set; }
    public int MaxOccupancy { get; set; }
    public string Description { get; set; } = string.Empty;
}
