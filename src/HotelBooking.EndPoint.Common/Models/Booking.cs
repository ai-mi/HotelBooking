using HotelBooking.EndPoint.Common.Enums;

namespace HotelBooking.EndPoint.Common.Models;

public class Booking : Base
{
    public string BookingReference { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public Guid RoomId { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
    public BookingStatus Status { get; set; }
    public BookingSource Source { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal? DiscountAmount { get; set; }
    public string? SpecialRequests { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    
    public Customer Customer { get; set; } = null!;
    public Room Room { get; set; } = null!;
    public Payment? Payment { get; set; }
    public ICollection<BookingAuditLog> AuditLogs { get; set; } = new List<BookingAuditLog>();
}
