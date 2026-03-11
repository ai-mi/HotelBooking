using HotelBooking.EndPoint.Common.Enums;

namespace HotelBooking.EndPoint.Common.Models;

public class Room : Base
{
    public string RoomNumber { get; set; } = string.Empty;
    public Guid HotelId { get; set; }
    public RoomCategory Category { get; set; }
    public decimal PricePerNight { get; set; }
    public int Floor { get; set; }
    public int MaxOccupancy { get; set; }
    public RoomStatus Status { get; set; }
    public string Description { get; set; } = string.Empty;
    
    public Hotel Hotel { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<RoomAuditLog> AuditLogs { get; set; } = new List<RoomAuditLog>();
}
