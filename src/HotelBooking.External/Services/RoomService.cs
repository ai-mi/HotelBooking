using HotelBooking.EndPoint.Common.Dto;
using HotelBooking.EndPoint.Common.Enums;
using HotelBooking.EndPoint.Common.Interfaces;
using HotelBooking.EndPoint.Common.Models;
using HotelBooking.External.Interfaces;

namespace HotelBooking.External.Services
{
	public class RoomService : IRoomService
	{
		private readonly IUnitOfWork _unitOfWork;

		public RoomService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<IEnumerable<RoomDto>> GetAllRoomsAsync()
		{
			var rooms = await _unitOfWork.Rooms.GetAllAsync();
			var roomDtos = new List<RoomDto>();

			foreach (var room in rooms)
			{
				roomDtos.Add(await MapToRoomDtoAsync(room));
			}

			return roomDtos;
		}

		public async Task<IEnumerable<AvailableRoomDto>> SearchAvailableRoomsAsync(RoomAvailabilitySearchDto searchDto)
		{
			var query = await GetAllRoomsAsync();

			// Filter by hotel if specified
			if (searchDto.HotelId.HasValue)
			{
				query = query.Where(r => r.HotelId == searchDto.HotelId.Value);
			}

			// Filter by category if specified
			if (searchDto.Category.HasValue)
			{
				query = query.Where(r => r.Category == searchDto.Category.Value);
			}

			// Filter by occupancy
			query = query.Where(r => r.MaxOccupancy >= searchDto.NumberOfGuests);

			// Filter by status
			query = query.Where(r => r.Status == RoomStatus.Available);

			var availableRooms = new List<AvailableRoomDto>();

			foreach (var room in query)
			{
				// Check if room is available for the specified dates
				if (await IsRoomAvailableAsync(room.Id, searchDto.CheckInDate, searchDto.CheckOutDate))
				{
					var nights = searchDto.CheckOutDate.DayNumber - searchDto.CheckInDate.DayNumber;
					var totalPrice = room.PricePerNight * nights;

					availableRooms.Add(new AvailableRoomDto
					{
						RoomId = room.Id,
						RoomNumber = room.RoomNumber,
						Category = room.Category,
						PricePerNight = room.PricePerNight,
						MaxOccupancy = room.MaxOccupancy,
						Description = room.Description,
						TotalPrice = totalPrice
					});
				}
			}

			return availableRooms.OrderBy(r => r.TotalPrice);
		}


		public async Task<IEnumerable<RoomDto>> GetRoomsByCategoryAsync(RoomCategory category)
		{
			var rooms = await _unitOfWork.Rooms.FindAsync(r => r.Category == category);
			var roomDtos = new List<RoomDto>();

			foreach (var room in rooms)
			{
				roomDtos.Add(await MapToRoomDtoAsync(room));
			}

			return roomDtos;
		}

		public async Task<bool> IsRoomAvailableAsync(Guid roomId, DateOnly checkIn, DateOnly checkOut)
		{
			var conflictingBookings = await _unitOfWork.Bookings.FindAsync(b =>
				b.RoomId == roomId &&
				b.Status != BookingStatus.Cancelled &&
				((b.CheckInDate < checkOut && b.CheckOutDate > checkIn)));

			return !conflictingBookings.Any();
		}


		private async Task<RoomDto> MapToRoomDtoAsync(Room room)
		{
			var hotel = await _unitOfWork.Hotels.GetByIdAsync(room.HotelId);

			return new RoomDto
			{
				Id = room.Id,
				RoomNumber = room.RoomNumber,
				HotelId = room.HotelId,
				HotelName = hotel?.Name ?? "Unknown",
				Category = room.Category,
				PricePerNight = room.PricePerNight,
				Floor = room.Floor,
				MaxOccupancy = room.MaxOccupancy,
				Status = room.Status,
				Description = room.Description
			};
		}

		public async Task<RoomDto?> GetRoomByIdAsync(Guid id)
		{
			var room = await _unitOfWork.Rooms.GetByIdAsync(id);
			if (room == null)
				return null;

			return await MapToRoomDtoAsync(room);
		}

		public async Task<IEnumerable<RoomDto>> GetRoomsByHotelAsync(Guid hotelId)
		{
			var rooms = await _unitOfWork.Rooms.FindAsync(r => r.HotelId == hotelId);
			var roomDtos = new List<RoomDto>();

			foreach (var room in rooms)
			{
				roomDtos.Add(await MapToRoomDtoAsync(room));
			}

			return roomDtos;
		}

		public async Task<IEnumerable<RoomDto>> GetRoomsByStatusAsync(RoomStatus status)
		{
			var rooms = await _unitOfWork.Rooms.FindAsync(r => r.Status == status);
			var roomDtos = new List<RoomDto>();

			foreach (var room in rooms)
			{
				roomDtos.Add(await MapToRoomDtoAsync(room));
			}

			return roomDtos;
		}

		public async Task<RoomDto> CreateRoomAsync(CreateRoomDto createRoomDto)
		{
			// Validate hotel exists
			var hotel = await _unitOfWork.Hotels.GetByIdAsync(createRoomDto.HotelId);
			if (hotel == null)
				throw new ArgumentException("Hotel not found");

			// Check if room number already exists in hotel
			var existingRoom = await _unitOfWork.Rooms.FindAsync(r =>
				r.HotelId == createRoomDto.HotelId &&
				r.RoomNumber == createRoomDto.RoomNumber);

			if (existingRoom.Any())
				throw new InvalidOperationException("Room number already exists in this hotel");

			var room = new Room
			{
				Id = Guid.NewGuid(),
				RoomNumber = createRoomDto.RoomNumber,
				HotelId = createRoomDto.HotelId,
				Category = createRoomDto.Category,
				PricePerNight = createRoomDto.PricePerNight,
				Floor = createRoomDto.Floor,
				MaxOccupancy = createRoomDto.MaxOccupancy,
				Status = RoomStatus.Available,
				Description = createRoomDto.Description,
				CreatedAt = DateTime.UtcNow
			};

			await _unitOfWork.Rooms.AddAsync(room);

			// Create audit log
			var auditLog = new RoomAuditLog
			{
				Id = Guid.NewGuid(),
				RoomId = room.Id,
				Action = "Created",
				Details = $"Room {room.RoomNumber} created",
				Timestamp = DateTime.UtcNow,
				CreatedAt = DateTime.UtcNow
			};
			await _unitOfWork.RoomAuditLogs.AddAsync(auditLog);

			await _unitOfWork.SaveChangesAsync();

			return await MapToRoomDtoAsync(room);
		}

		public async Task<RoomDto> UpdateRoomAsync(Guid id, CreateRoomDto updateRoomDto)
		{
			var room = await _unitOfWork.Rooms.GetByIdAsync(id);
			if (room == null)
				throw new ArgumentException("Room not found");

			room.RoomNumber = updateRoomDto.RoomNumber;
			room.Category = updateRoomDto.Category;
			room.PricePerNight = updateRoomDto.PricePerNight;
			room.Floor = updateRoomDto.Floor;
			room.MaxOccupancy = updateRoomDto.MaxOccupancy;
			room.Description = updateRoomDto.Description;
			room.UpdatedAt = DateTime.UtcNow;

			await _unitOfWork.Rooms.UpdateAsync(room);

			// Create audit log
			var auditLog = new RoomAuditLog
			{
				Id = Guid.NewGuid(),
				RoomId = room.Id,
				Action = "Updated",
				Details = "Room details updated",
				Timestamp = DateTime.UtcNow,
				CreatedAt = DateTime.UtcNow
			};
			await _unitOfWork.RoomAuditLogs.AddAsync(auditLog);

			await _unitOfWork.SaveChangesAsync();

			return await MapToRoomDtoAsync(room);
		}

		public async Task<bool> UpdateRoomStatusAsync(Guid id, RoomStatus status)
		{
			var room = await _unitOfWork.Rooms.GetByIdAsync(id);
			if (room == null)
				return false;

			var oldStatus = room.Status;
			room.Status = status;
			room.UpdatedAt = DateTime.UtcNow;

			await _unitOfWork.Rooms.UpdateAsync(room);

			// Create audit log
			var auditLog = new RoomAuditLog
			{
				Id = Guid.NewGuid(),
				RoomId = room.Id,
				Action = "StatusChanged",
				Details = $"Status changed from {oldStatus} to {status}",
				Timestamp = DateTime.UtcNow,
				CreatedAt = DateTime.UtcNow
			};
			await _unitOfWork.RoomAuditLogs.AddAsync(auditLog);

			await _unitOfWork.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteRoomAsync(Guid id)
		{
			var room = await _unitOfWork.Rooms.GetByIdAsync(id);
			if (room == null)
				return false;

			// Check if room has active bookings
			var activeBookings = await _unitOfWork.Bookings.FindAsync(b =>
				b.RoomId == id &&
				(b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.CheckedIn));

			if (activeBookings.Any())
				throw new InvalidOperationException("Cannot delete room with active bookings");

			await _unitOfWork.Rooms.DeleteAsync(room);
			await _unitOfWork.SaveChangesAsync();
			return true;
		}

		public async Task<RoomHistoryDto> GetRoomHistoryAsync(Guid roomId, DateOnly? startDate = null, DateOnly? endDate = null)
		{
			var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
			if (room == null)
				throw new ArgumentException("Room not found");

			var bookingsQuery = await _unitOfWork.Bookings.FindAsync(b => b.RoomId == roomId);

			if (startDate.HasValue)
				bookingsQuery = bookingsQuery.Where(b => b.CheckInDate >= startDate.Value);

			if (endDate.HasValue)
				bookingsQuery = bookingsQuery.Where(b => b.CheckOutDate <= endDate.Value);

			var bookings = bookingsQuery.OrderByDescending(b => b.CheckInDate).ToList();

			var bookingHistoryItems = new List<BookingHistoryItem>();
			foreach (var booking in bookings)
			{
				var customer = await _unitOfWork.Customers.GetByIdAsync(booking.CustomerId);
				bookingHistoryItems.Add(new BookingHistoryItem
				{
					BookingReference = booking.BookingReference,
					CustomerName = customer != null ? $"{customer.FirstName} {customer.LastName}" : "Unknown",
					CheckInDate = booking.CheckInDate,
					CheckOutDate = booking.CheckOutDate,
					Status = booking.Status.ToString()
				});
			}

			var auditLogsQuery = await _unitOfWork.RoomAuditLogs.FindAsync(a => a.RoomId == roomId);

			if (startDate.HasValue)
				auditLogsQuery = auditLogsQuery.Where(a => a.Timestamp >= startDate.Value.ToDateTime(TimeOnly.MinValue));

			if (endDate.HasValue)
				auditLogsQuery = auditLogsQuery.Where(a => a.Timestamp <= endDate.Value.ToDateTime(TimeOnly.MaxValue));

			var auditLogs = auditLogsQuery.OrderByDescending(a => a.Timestamp).ToList();

			var auditLogItems = auditLogs.Select(a => new AuditLogItem
			{
				Timestamp = a.Timestamp,
				Action = a.Action,
				Details = a.Details,
				PerformedBy = a.PerformedBy
			}).ToList();

			var totalBookings = bookings.Count();
			var cancelledBookings = bookings.Count(b => b.Status == BookingStatus.Cancelled);

			return new RoomHistoryDto
			{
				RoomId = room.Id,
				RoomNumber = room.RoomNumber,
				Bookings = bookingHistoryItems,
				AuditLogs = auditLogItems,
				Statistics = new RoomStatistics
				{
					TotalBookings = totalBookings,
					CancelledBookings = cancelledBookings,
					OccupancyRate = totalBookings > 0 ? (double)(totalBookings - cancelledBookings) / totalBookings * 100 : 0,
					UtilizationRate = 0 // Can be calculated based on date range
				}
			};
		}
	}
}
