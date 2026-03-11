using HotelBooking.EndPoint.Common.Interfaces;
using HotelBooking.EndPoint.Common.Models;
using HotelBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly HotelBookingDbContext _context;

		public UnitOfWork(HotelBookingDbContext context)
		{
			_context = context;
			Hotels = new Repository<Hotel>(_context);
			Rooms = new Repository<Room>(_context);
			Customers = new Repository<Customer>(_context);
			Bookings = new Repository<Booking>(_context);
			Payments = new Repository<Payment>(_context);
			LoyaltyMembers = new Repository<LoyaltyMember>(_context);
		}

		public IRepository<Hotel> Hotels { get; }
		public IRepository<Room> Rooms { get; }
		public IRepository<Customer> Customers { get; }
		public IRepository<Booking> Bookings { get; }
		public IRepository<Payment> Payments { get; }
		public IRepository<LoyaltyMember> LoyaltyMembers { get; }

		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
