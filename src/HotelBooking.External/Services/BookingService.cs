using HotelBooking.EndPoint.Common.Dto;
using HotelBooking.EndPoint.Common.Enums;
using HotelBooking.EndPoint.Common.Interfaces;
using HotelBooking.EndPoint.Common.Models;
using HotelBooking.External.Interfaces;

namespace HotelBooking.External.Services
{
	public class BookingService : IBookingService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILoyaltyService _loyaltyService;

		public BookingService(IUnitOfWork unitOfWork, ILoyaltyService loyaltyService)
		{
			_unitOfWork = unitOfWork;
			_loyaltyService = loyaltyService;
		}

		public async Task<BookingDto?> GetBookingByIdAsync(Guid id)
		{
			var bookings = await _unitOfWork.Bookings.FindAsync(b => b.Id == id);
			var booking = bookings.FirstOrDefault();

			if (booking == null)
				return null;

			return await MapToBookingDtoAsync(booking);
		}

		public async Task<BookingDto?> GetBookingByReferenceAsync(string bookingReference)
		{
			var bookings = await _unitOfWork.Bookings.FindAsync(b => b.BookingReference == bookingReference);
			var booking = bookings.FirstOrDefault();

			if (booking == null)
				return null;

			return await MapToBookingDtoAsync(booking);
		}

		public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
		{
			var bookings = await _unitOfWork.Bookings.GetAllAsync();
			var bookingDtos = new List<BookingDto>();

			foreach (var booking in bookings)
			{
				bookingDtos.Add(await MapToBookingDtoAsync(booking));
			}

			return bookingDtos;
		}

		public async Task<IEnumerable<BookingDto>> GetBookingsByCustomerAsync(Guid customerId)
		{
			var bookings = await _unitOfWork.Bookings.FindAsync(b => b.CustomerId == customerId);
			var bookingDtos = new List<BookingDto>();

			foreach (var booking in bookings)
			{
				bookingDtos.Add(await MapToBookingDtoAsync(booking));
			}

			return bookingDtos;
		}

		public async Task<IEnumerable<BookingDto>> GetBookingsByRoomAsync(Guid roomId)
		{
			var bookings = await _unitOfWork.Bookings.FindAsync(b => b.RoomId == roomId);
			var bookingDtos = new List<BookingDto>();

			foreach (var booking in bookings)
			{
				bookingDtos.Add(await MapToBookingDtoAsync(booking));
			}

			return bookingDtos;
		}

		public async Task<IEnumerable<BookingDto>> GetBookingsByDateRangeAsync(DateOnly startDate, DateOnly endDate)
		{
			var bookings = await _unitOfWork.Bookings.FindAsync(b =>
				b.CheckInDate >= startDate && b.CheckOutDate <= endDate);

			var bookingDtos = new List<BookingDto>();

			foreach (var booking in bookings)
			{
				bookingDtos.Add(await MapToBookingDtoAsync(booking));
			}

			return bookingDtos;
		}

		public async Task<BookingDto> CreateBookingAsync(CreateBookingDto createBookingDto)
		{
			// Validate customer exists
			var customer = await _unitOfWork.Customers.GetByIdAsync(createBookingDto.CustomerId);
			if (customer == null)
				throw new ArgumentException("Customer not found");

			// Determine room
			Guid roomId;
			if (createBookingDto.RoomId.HasValue)
			{
				roomId = createBookingDto.RoomId.Value;
			}
			else if (createBookingDto.RoomCategory.HasValue)
			{
				// Find available room by category
				var rooms = await _unitOfWork.Rooms.FindAsync(r =>
					r.HotelId == createBookingDto.HotelId &&
					r.Category == createBookingDto.RoomCategory.Value &&
					r.Status == RoomStatus.Available);

				var availableRoom = rooms.FirstOrDefault();
				if (availableRoom == null)
					throw new ArgumentException("No available rooms in the requested category");

				roomId = availableRoom.Id;
			}
			else
			{
				throw new ArgumentException("Either RoomId or RoomCategory must be provided");
			}

			// Validate room exists and is available
			var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
			if (room == null)
				throw new ArgumentException("Room not found");

			// Check for double booking
			var conflictingBookings = await _unitOfWork.Bookings.FindAsync(b =>
				b.RoomId == roomId &&
				b.Status != BookingStatus.Cancelled &&
				((b.CheckInDate < createBookingDto.CheckOutDate && b.CheckOutDate > createBookingDto.CheckInDate)));

			if (conflictingBookings.Any())
				throw new InvalidOperationException("Room is not available for the selected dates");

			// Calculate price
			var totalPrice = await CalculateTotalPriceAsync(roomId, createBookingDto.CheckInDate,
				createBookingDto.CheckOutDate, createBookingDto.CustomerId);

			// Create booking
			var booking = new Booking
			{
				Id = Guid.NewGuid(),
				BookingReference = GenerateBookingReference(),
				CustomerId = createBookingDto.CustomerId,
				RoomId = roomId,
				CheckInDate = createBookingDto.CheckInDate,
				CheckOutDate = createBookingDto.CheckOutDate,
				NumberOfGuests = createBookingDto.NumberOfGuests,
				Status = BookingStatus.Confirmed,
				Source = createBookingDto.Source,
				TotalPrice = totalPrice,
				SpecialRequests = createBookingDto.SpecialRequests,
				CreatedAt = DateTime.UtcNow
			};

			// Calculate discount if customer is loyalty member
			var loyaltyMember = await _loyaltyService.GetLoyaltyMemberByCustomerIdAsync(createBookingDto.CustomerId);
			if (loyaltyMember != null)
			{
				var discount = await _loyaltyService.CalculateDiscountAsync(createBookingDto.CustomerId, totalPrice);
				booking.DiscountAmount = discount;
				booking.TotalPrice -= discount;
			}

			await _unitOfWork.Bookings.AddAsync(booking);

			//// Create audit log
			var auditLog = new BookingAuditLog
			{
				Id = Guid.NewGuid(),
				BookingId = booking.Id,
				Action = "Created",
				Details = $"Booking created for {createBookingDto.NumberOfGuests} guests",
				Timestamp = DateTime.UtcNow,
				CreatedAt = DateTime.UtcNow
			};
			await _unitOfWork.BookingAuditLogs.AddAsync(auditLog);

			//// Add loyalty points if member
			if (loyaltyMember != null)
			{
				var pointsToAdd = (int)(totalPrice / 10); // 1 point per $10
				await _loyaltyService.AddPointsAsync(loyaltyMember.Id, pointsToAdd,
					$"Booking {booking.BookingReference}", booking.Id);
			}

			await _unitOfWork.SaveChangesAsync();

			return await MapToBookingDtoAsync(booking);
		}

		public async Task<BookingDto> UpdateBookingAsync(Guid id, CreateBookingDto updateBookingDto)
		{
			var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
			if (booking == null)
				throw new ArgumentException("Booking not found");

			if (booking.Status == BookingStatus.Cancelled)
				throw new InvalidOperationException("Cannot update cancelled booking");

			// Update fields
			booking.CheckInDate = updateBookingDto.CheckInDate;
			booking.CheckOutDate = updateBookingDto.CheckOutDate;
			booking.NumberOfGuests = updateBookingDto.NumberOfGuests;
			booking.SpecialRequests = updateBookingDto.SpecialRequests;
			booking.UpdatedAt = DateTime.UtcNow;

			// Recalculate price
			booking.TotalPrice = await CalculateTotalPriceAsync(booking.RoomId,
				updateBookingDto.CheckInDate, updateBookingDto.CheckOutDate, booking.CustomerId);

			await _unitOfWork.Bookings.UpdateAsync(booking);

			//// Create audit log
			var auditLog = new BookingAuditLog
			{
				Id = Guid.NewGuid(),
				BookingId = booking.Id,
				Action = "Updated",
				Details = "Booking details updated",
				Timestamp = DateTime.UtcNow,
				CreatedAt = DateTime.UtcNow
			};
			await _unitOfWork.BookingAuditLogs.AddAsync(auditLog);

			await _unitOfWork.SaveChangesAsync();

			return await MapToBookingDtoAsync(booking);
		}

		public async Task<bool> CancelBookingAsync(Guid id, string cancellationReason)
		{
			var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
			if (booking == null)
				return false;

			if (booking.Status == BookingStatus.Cancelled)
				return false;

			booking.Status = BookingStatus.Cancelled;
			booking.CancelledAt = DateTime.UtcNow;
			booking.CancellationReason = cancellationReason;
			booking.UpdatedAt = DateTime.UtcNow;

			await _unitOfWork.Bookings.UpdateAsync(booking);

			//// Create audit log
			var auditLog = new BookingAuditLog
			{
				Id = Guid.NewGuid(),
				BookingId = booking.Id,
				Action = "Cancelled",
				Details = $"Reason: {cancellationReason}",
				Timestamp = DateTime.UtcNow,
				CreatedAt = DateTime.UtcNow
			};
			await _unitOfWork.BookingAuditLogs.AddAsync(auditLog);

			await _unitOfWork.SaveChangesAsync();
			return true;
		}

		public async Task<bool> CheckInBookingAsync(Guid id)
		{
			var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
			if (booking == null || booking.Status != BookingStatus.Confirmed)
				return false;

			booking.Status = BookingStatus.CheckedIn;
			booking.UpdatedAt = DateTime.UtcNow;

			await _unitOfWork.Bookings.UpdateAsync(booking);

			var auditLog = new BookingAuditLog
			{
				Id = Guid.NewGuid(),
				BookingId = booking.Id,
				Action = "CheckedIn",
				Details = "Guest checked in",
				Timestamp = DateTime.UtcNow,
				CreatedAt = DateTime.UtcNow
			};
			await _unitOfWork.BookingAuditLogs.AddAsync(auditLog);

			await _unitOfWork.SaveChangesAsync();
			return true;
		}

		public async Task<bool> CheckOutBookingAsync(Guid id)
		{
			var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
			if (booking == null || booking.Status != BookingStatus.CheckedIn)
				return false;

			booking.Status = BookingStatus.CheckedOut;
			booking.UpdatedAt = DateTime.UtcNow;

			await _unitOfWork.Bookings.UpdateAsync(booking);

			var auditLog = new BookingAuditLog
			{
				Id = Guid.NewGuid(),
				BookingId = booking.Id,
				Action = "CheckedOut",
				Details = "Guest checked out",
				Timestamp = DateTime.UtcNow,
				CreatedAt = DateTime.UtcNow
			};
			await _unitOfWork.BookingAuditLogs.AddAsync(auditLog);

			await _unitOfWork.SaveChangesAsync();
			return true;
		}

		public async Task<decimal> CalculateTotalPriceAsync(Guid roomId, DateOnly checkIn, DateOnly checkOut, Guid? customerId = null)
		{
			var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
			if (room == null)
				throw new ArgumentException("Room not found");

			var nights = checkOut.DayNumber - checkIn.DayNumber;
			if (nights <= 0)
				throw new ArgumentException("Check-out date must be after check-in date");

			var basePrice = room.PricePerNight * nights;

			return basePrice;
		}

		private async Task<BookingDto> MapToBookingDtoAsync(Booking booking)
		{
			var customer = await _unitOfWork.Customers.GetByIdAsync(booking.CustomerId);
			var room = await _unitOfWork.Rooms.GetByIdAsync(booking.RoomId);

			return new BookingDto
			{
				Id = booking.Id,
				BookingReference = booking.BookingReference,
				CustomerId = booking.CustomerId,
				CustomerName = customer != null ? $"{customer.FirstName} {customer.LastName}" : "Unknown",
				RoomId = booking.RoomId,
				RoomNumber = room?.RoomNumber ?? "Unknown",
				RoomCategory = room?.Category ?? RoomCategory.Standard,
				CheckInDate = booking.CheckInDate,
				CheckOutDate = booking.CheckOutDate,
				NumberOfGuests = booking.NumberOfGuests,
				Status = booking.Status,
				Source = booking.Source,
				TotalPrice = booking.TotalPrice,
				DiscountAmount = booking.DiscountAmount,
				SpecialRequests = booking.SpecialRequests,
				CreatedAt = booking.CreatedAt
			};
		}

		private string GenerateBookingReference()
		{
			return $"BK{DateTime.UtcNow:yyyyMMdd}{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
		}

	}
}
