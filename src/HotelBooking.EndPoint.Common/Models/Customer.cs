namespace HotelBooking.EndPoint.Common.Models;

public class Customer : Base
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? PassportNumber { get; set; }
    public bool IsActive { get; set; }
    
    public LoyaltyMember? LoyaltyMember { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<CustomerAuditLog> AuditLogs { get; set; } = new List<CustomerAuditLog>();
}
