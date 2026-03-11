using HotelBooking.EndPoint.Common.Enums;

namespace HotelBooking.EndPoint.Common.Models;

public class Payment : Base
{
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public string? TransactionId { get; set; }
    public DateTime? PaidAt { get; set; }
    
    public Booking Booking { get; set; } = null!;
}
