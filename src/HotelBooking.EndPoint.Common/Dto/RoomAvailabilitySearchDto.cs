using HotelBooking.EndPoint.Common.Enums;

namespace HotelBooking.EndPoint.Common.Dto;

public class RoomAvailabilitySearchDto
{
    public Guid? HotelId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public RoomCategory? Category { get; set; }
    public int NumberOfGuests { get; set; }
}
