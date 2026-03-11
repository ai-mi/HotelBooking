using HotelBooking.EndPoint.Common.Enums;

namespace HotelBooking.EndPoint.Common.Dto;

public class BookingDto
{
    public Guid Id { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public RoomCategory RoomCategory { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
    public BookingStatus Status { get; set; }
    public BookingSource Source { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal? DiscountAmount { get; set; }
    public string? SpecialRequests { get; set; }
    public DateTime CreatedAt { get; set; }
}
