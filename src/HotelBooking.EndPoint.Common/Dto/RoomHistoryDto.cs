namespace HotelBooking.EndPoint.Common.Dto;

public class RoomHistoryDto
{
    public Guid RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public List<BookingHistoryItem> Bookings { get; set; } = new();
    public List<AuditLogItem> AuditLogs { get; set; } = new();
    public RoomStatistics Statistics { get; set; } = new();
}

public class BookingHistoryItem
{
    public string BookingReference { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class AuditLogItem
{
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string? PerformedBy { get; set; }
}

public class RoomStatistics
{
    public int TotalBookings { get; set; }
    public int CancelledBookings { get; set; }
    public double OccupancyRate { get; set; }
    public double UtilizationRate { get; set; }
}
