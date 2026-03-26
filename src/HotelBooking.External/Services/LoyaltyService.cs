using HotelBooking.EndPoint.Common.Dto;
using HotelBooking.EndPoint.Common.Interfaces;
using HotelBooking.EndPoint.Common.Models;
using HotelBooking.External.Interfaces;

namespace HotelBooking.External.Services
{
	public class LoyaltyService : ILoyaltyService
	{
		private readonly IUnitOfWork _unitOfWork;

		public LoyaltyService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public Task<int> AddPointsAsync(Guid loyaltyMemberId, int points, string description, Guid? bookingId = null)
		{
			throw new NotImplementedException();
		}

		public Task<decimal> CalculateDiscountAsync(Guid customerId, decimal originalPrice)
		{
			throw new NotImplementedException();
		}

		public async Task<LoyaltyMemberDto?> GetLoyaltyMemberByCustomerIdAsync(Guid customerId)
		{
			var loyaltyMembers = await _unitOfWork.LoyaltyMembers.FindAsync(l => l.CustomerId == customerId);
			var loyaltyMember = loyaltyMembers.FirstOrDefault();

			if (loyaltyMember == null)
				return null;

			return await MapToLoyaltyMemberDtoAsync(loyaltyMember);
		}

		public async Task<IEnumerable<LoyaltyTransactionDto>> GetTransactionHistoryAsync(Guid loyaltyMemberId, int count = 10)
		{
			var transactions = await _unitOfWork.LoyaltyTransactions.FindAsync(t => t.LoyaltyMemberId == loyaltyMemberId);

			return transactions
				.OrderByDescending(t => t.CreatedAt)
				.Take(count)
				.Select(t => new LoyaltyTransactionDto
				{
					Date = t.CreatedAt,
					Points = t.Points,
					Type = t.TransactionType,
					Description = t.Description
				})
				.ToList();
		}

		private async Task<LoyaltyMemberDto> MapToLoyaltyMemberDtoAsync(LoyaltyMember loyaltyMember)
		{
			var recentTransactions = await GetTransactionHistoryAsync(loyaltyMember.Id, 5);

			return new LoyaltyMemberDto
			{
				Id = loyaltyMember.Id,
				MembershipNumber = loyaltyMember.MembershipNumber,
				Tier = loyaltyMember.Tier,
				Points = loyaltyMember.Points,
				JoinedAt = loyaltyMember.JoinedAt,
				RecentTransactions = recentTransactions.ToList()
			};
		}

	}


}
