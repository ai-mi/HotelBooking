using HotelBooking.EndPoint.Common.Enums;

namespace HotelBooking.EndPoint.Common.Dto;

public class RoomAvailabilitySearchDto
{
    public Guid? HotelId { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public RoomCategory? Category { get; set; }
    public int NumberOfGuests { get; set; }
}
