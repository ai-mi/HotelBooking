namespace HotelBooking.EndPoint.Common.Models;

public class BookingAuditLog : Base
{
    public Guid BookingId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string? PerformedBy { get; set; }
    public DateTime Timestamp { get; set; }
    
    public Booking Booking { get; set; } = null!;
}
