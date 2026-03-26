using HotelBooking.EndPoint.Common.Models;

namespace HotelBooking.EndPoint.Common.Interfaces
{
	public interface IUnitOfWork : IDisposable
	{
		IRepository<Hotel> Hotels { get; }
		IRepository<Room> Rooms { get; }
		IRepository<Customer> Customers { get; }
		IRepository<Booking> Bookings { get; }
		IRepository<Payment> Payments { get; }
		IRepository<LoyaltyMember> LoyaltyMembers { get; }
		IRepository<LoyaltyTransaction> LoyaltyTransactions { get; }
		IRepository<RoomAuditLog> RoomAuditLogs { get; }
		IRepository<BookingAuditLog> BookingAuditLogs { get; }
		IRepository<CustomerAuditLog> CustomerAuditLogs { get; }

		Task<int> SaveChangesAsync();
	}
}
