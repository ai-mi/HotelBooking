using HotelBooking.EndPoint.Common.Enums;
using HotelBooking.EndPoint.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Data
{
	public static class SeedData
	{
		public static async Task SeedAsync(HotelBookingDbContext context)
		{
			// Check if data already exists
			if (context.Hotels.Any())
				return;

			// Seed Hotels
			var hotel1 = new Hotel
			{
				Id = Guid.NewGuid(),
				Name = "Grand Plaza Hotel",
				Address = "123 Main Street",
				City = "New York",
				Country = "USA",
				PhoneNumber = "+1-555-0100",
				Email = "info@grandplaza.com",
				IsActive = true,
				CreatedAt = DateTime.UtcNow
			};

			var hotel2 = new Hotel
			{
				Id = Guid.NewGuid(),
				Name = "Seaside Resort",
				Address = "456 Beach Avenue",
				City = "Miami",
				Country = "USA",
				PhoneNumber = "+1-555-0200",
				Email = "info@seasideresort.com",
				IsActive = true,
				CreatedAt = DateTime.UtcNow
			};

			await context.Hotels.AddRangeAsync(hotel1, hotel2);
			await context.SaveChangesAsync();

			// Seed Rooms for Hotel 1
			var rooms = new List<Room>
		{
			new Room
			{
				Id = Guid.NewGuid(),
				RoomNumber = "101",
				HotelId = hotel1.Id,
				Category = RoomCategory.Suite,
				PricePerNight = 100.00m,
				Floor = 1,
				MaxOccupancy = 1,
				Status = RoomStatus.Available,
				Description = "Cozy single room with city view",
				CreatedAt = DateTime.UtcNow
			},
			new Room
			{
				Id = Guid.NewGuid(),
				RoomNumber = "102",
				HotelId = hotel1.Id,
				Category = RoomCategory.Deluxe,
				PricePerNight = 150.00m,
				Floor = 1,
				MaxOccupancy = 2,
				Status = RoomStatus.Available,
				Description = "Comfortable double room with queen bed",
				CreatedAt = DateTime.UtcNow
			},
			new Room
			{
				Id = Guid.NewGuid(),
				RoomNumber = "201",
				HotelId = hotel1.Id,
				Category = RoomCategory.Suite,
				PricePerNight = 300.00m,
				Floor = 2,
				MaxOccupancy = 4,
				Status = RoomStatus.Available,
				Description = "Luxury suite with living room and kitchenette",
				CreatedAt = DateTime.UtcNow
			},
			new Room
			{
				Id = Guid.NewGuid(),
				RoomNumber = "301",
				HotelId = hotel1.Id,
				Category = RoomCategory.Standard,
				PricePerNight = 500.00m,
				Floor = 3,
				MaxOccupancy = 6,
				Status = RoomStatus.Available,
				Description = "Presidential suite with panoramic views",
				CreatedAt = DateTime.UtcNow
			}
		};

			// Seed Rooms for Hotel 2
			rooms.AddRange(new List<Room>
		{
			new Room
			{
				Id = Guid.NewGuid(),
				RoomNumber = "101",
				HotelId = hotel2.Id,
				Category = RoomCategory.Family,
				PricePerNight = 180.00m,
				Floor = 1,
				MaxOccupancy = 2,
				Status = RoomStatus.Available,
				Description = "Ocean view double room",
				CreatedAt = DateTime.UtcNow
			},
			new Room
			{
				Id = Guid.NewGuid(),
				RoomNumber = "102",
				HotelId = hotel2.Id,
				Category = RoomCategory.Deluxe,
				PricePerNight = 250.00m,
				Floor = 1,
				MaxOccupancy = 3,
				Status = RoomStatus.Available,
				Description = "Deluxe room with beachfront access",
				CreatedAt = DateTime.UtcNow
			}
		});

			await context.Rooms.AddRangeAsync(rooms);
			await context.SaveChangesAsync();

			// Seed Sample Customer
			var customer = new Customer
			{
				Id = Guid.NewGuid(),
				FirstName = "John",
				LastName = "Doe",
				Email = "john.doe@email.com",
				PhoneNumber = "+1-555-1234",
				Address = "789 Customer Street",
				City = "Boston",
				Country = "USA",
				DateOfBirth = new DateTime(1990, 5, 15),
				IsActive = true,
				CreatedAt = DateTime.UtcNow
			};

			await context.Customers.AddAsync(customer);
			await context.SaveChangesAsync();

			Console.WriteLine("Seed data created successfully!");
		}
	}

}
