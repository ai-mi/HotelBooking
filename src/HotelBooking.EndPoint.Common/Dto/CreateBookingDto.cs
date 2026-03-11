using HotelBooking.EndPoint.Common.Enums;

namespace HotelBooking.EndPoint.Common.Dto;

public class CreateBookingDto
{
    public Guid CustomerId { get; set; }
    public Guid? RoomId { get; set; }
    public RoomCategory? RoomCategory { get; set; }
    public Guid HotelId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
    public BookingSource Source { get; set; }
    public string? SpecialRequests { get; set; }
}
