namespace HotelBooking.EndPoint.Common.Models;

public class RoomAuditLog : Base
{
    public Guid RoomId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string? PerformedBy { get; set; }
    public DateTime Timestamp { get; set; }
    
    public Room Room { get; set; } = null!;
}
