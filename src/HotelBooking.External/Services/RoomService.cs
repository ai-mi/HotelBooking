using HotelBooking.EndPoint.Common.Dto;
using HotelBooking.EndPoint.Common.Enums;
using HotelBooking.EndPoint.Common.Interfaces;
using HotelBooking.EndPoint.Common.Models;
using HotelBooking.External.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	}
}
