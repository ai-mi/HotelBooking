using HotelBooking.EndPoint.Common.Dto;

namespace HotelBooking.External.Interfaces
{
	public interface ILoyaltyService
	{
		Task<LoyaltyMemberDto?> GetLoyaltyMemberByCustomerIdAsync(Guid customerId);
		Task<int> AddPointsAsync(Guid loyaltyMemberId, int points, string description, Guid? bookingId = null);
		Task<decimal> CalculateDiscountAsync(Guid customerId, decimal originalPrice);
	}
}
