

using HotelBooking.EndPoint.Common.Enums;

namespace HotelBooking.EndPoint.Common.Dto;

public class LoyaltyMemberDto
{
    public Guid Id { get; set; }
    public string MembershipNumber { get; set; } = string.Empty;
    public LoyaltyTier Tier { get; set; }
    public int Points { get; set; }
    public DateTime JoinedAt { get; set; }
    public List<LoyaltyTransactionDto> RecentTransactions { get; set; } = new();
}

public class LoyaltyTransactionDto
{
    public DateTime Date { get; set; }
    public int Points { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
