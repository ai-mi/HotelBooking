using HotelBooking.EndPoint.Common.Enums;

namespace HotelBooking.EndPoint.Common.Models;

public class LoyaltyMember : Base
{
    public Guid CustomerId { get; set; }
    public string MembershipNumber { get; set; } = string.Empty;
    public LoyaltyTier Tier { get; set; }
    public int Points { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? TierUpdatedAt { get; set; }
    
    public Customer Customer { get; set; } = null!;
    public ICollection<LoyaltyTransaction> Transactions { get; set; } = new List<LoyaltyTransaction>();
}
