namespace HotelBooking.EndPoint.Common.Models;

public class LoyaltyTransaction : Base
{
    public Guid LoyaltyMemberId { get; set; }
    public int Points { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? BookingId { get; set; }
    
    public LoyaltyMember LoyaltyMember { get; set; } = null!;
}
