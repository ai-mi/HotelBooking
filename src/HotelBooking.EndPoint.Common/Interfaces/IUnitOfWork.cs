using HotelBooking.EndPoint.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	}
}
