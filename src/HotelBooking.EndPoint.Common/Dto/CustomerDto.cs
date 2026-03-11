namespace HotelBooking.EndPoint.Common.Dto;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsLoyaltyMember { get; set; }
    public string? LoyaltyTier { get; set; }
    public int? LoyaltyPoints { get; set; }
}
