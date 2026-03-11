namespace HotelBooking.EndPoint.Common.Models;

public class CustomerAuditLog : Base
{
    public Guid CustomerId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string? PerformedBy { get; set; }
    public DateTime Timestamp { get; set; }
    
    public Customer Customer { get; set; } = null!;
}
