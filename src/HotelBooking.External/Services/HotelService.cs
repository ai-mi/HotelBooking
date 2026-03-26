using HotelBooking.EndPoint.Common.Dto;
using HotelBooking.EndPoint.Common.Interfaces;
using HotelBooking.EndPoint.Common.Models;
using HotelBooking.External.Interfaces;

namespace HotelBooking.External.Services
{
	public class HotelService : IHotelService
	{
		private readonly IUnitOfWork _unitOfWork;

		public HotelService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<HotelDto?> GetHotelByIdAsync(Guid id)
		{
			var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
			if (hotel == null)
				return null;

			return await MapToHotelDtoAsync(hotel);
		}

		public async Task<IEnumerable<HotelDto>> GetAllHotelsAsync()
		{
			var hotels = await _unitOfWork.Hotels.GetAllAsync();
			var hotelDtos = new List<HotelDto>();

			foreach (var hotel in hotels)
			{
				hotelDtos.Add(await MapToHotelDtoAsync(hotel));
			}

			return hotelDtos;
		}

		public async Task<IEnumerable<HotelDto>> GetActiveHotelsAsync()
		{
			var hotels = await _unitOfWork.Hotels.FindAsync(h => h.IsActive);
			var hotelDtos = new List<HotelDto>();

			foreach (var hotel in hotels)
			{
				hotelDtos.Add(await MapToHotelDtoAsync(hotel));
			}

			return hotelDtos;
		}

		public async Task<IEnumerable<HotelDto>> GetHotelsByCityAsync(string city)
		{
			var hotels = await _unitOfWork.Hotels.FindAsync(h => h.City.ToLower() == city.ToLower());
			var hotelDtos = new List<HotelDto>();

			foreach (var hotel in hotels)
			{
				hotelDtos.Add(await MapToHotelDtoAsync(hotel));
			}

			return hotelDtos;
		}

		public async Task<IEnumerable<HotelDto>> GetHotelsByCountryAsync(string country)
		{
			var hotels = await _unitOfWork.Hotels.FindAsync(h => h.Country.ToLower() == country.ToLower());
			var hotelDtos = new List<HotelDto>();

			foreach (var hotel in hotels)
			{
				hotelDtos.Add(await MapToHotelDtoAsync(hotel));
			}

			return hotelDtos;
		}

		public async Task<HotelDto> CreateHotelAsync(CreateHotelDto createHotelDto)
		{
			var hotel = new Hotel
			{
				Id = Guid.NewGuid(),
				Name = createHotelDto.Name,
				Address = createHotelDto.Address,
				City = createHotelDto.City,
				Country = createHotelDto.Country,
				PhoneNumber = createHotelDto.PhoneNumber,
				Email = createHotelDto.Email,
				IsActive = true,
				CreatedAt = DateTime.UtcNow
			};

			await _unitOfWork.Hotels.AddAsync(hotel);
			await _unitOfWork.SaveChangesAsync();

			return await MapToHotelDtoAsync(hotel);
		}

		public async Task<HotelDto> UpdateHotelAsync(Guid id, CreateHotelDto updateHotelDto)
		{
			var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
			if (hotel == null)
				throw new ArgumentException("Hotel not found");

			hotel.Name = updateHotelDto.Name;
			hotel.Address = updateHotelDto.Address;
			hotel.City = updateHotelDto.City;
			hotel.Country = updateHotelDto.Country;
			hotel.PhoneNumber = updateHotelDto.PhoneNumber;
			hotel.Email = updateHotelDto.Email;
			hotel.UpdatedAt = DateTime.UtcNow;

			await _unitOfWork.Hotels.UpdateAsync(hotel);
			await _unitOfWork.SaveChangesAsync();

			return await MapToHotelDtoAsync(hotel);
		}

		public async Task<bool> DeactivateHotelAsync(Guid id)
		{
			var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
			if (hotel == null)
				return false;

			hotel.IsActive = false;
			hotel.UpdatedAt = DateTime.UtcNow;

			await _unitOfWork.Hotels.UpdateAsync(hotel);
			await _unitOfWork.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ActivateHotelAsync(Guid id)
		{
			var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
			if (hotel == null)
				return false;

			hotel.IsActive = true;
			hotel.UpdatedAt = DateTime.UtcNow;

			await _unitOfWork.Hotels.UpdateAsync(hotel);
			await _unitOfWork.SaveChangesAsync();
			return true;
		}

		public async Task<IEnumerable<RoomDto>> GetHotelRoomsAsync(Guid hotelId)
		{
			var rooms = await _unitOfWork.Rooms.FindAsync(r => r.HotelId == hotelId);
			var roomDtos = new List<RoomDto>();

			foreach (var room in rooms)
			{
				var hotel = await _unitOfWork.Hotels.GetByIdAsync(room.HotelId);
				roomDtos.Add(new RoomDto
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
				});
			}

			return roomDtos.OrderBy(r => r.RoomNumber);
		}

		private async Task<HotelDto> MapToHotelDtoAsync(Hotel hotel)
		{
			var rooms = await _unitOfWork.Rooms.FindAsync(r => r.HotelId == hotel.Id);

			return new HotelDto
			{
				Id = hotel.Id,
				Name = hotel.Name,
				Address = hotel.Address,
				City = hotel.City,
				Country = hotel.Country,
				PhoneNumber = hotel.PhoneNumber,
				Email = hotel.Email,
				IsActive = hotel.IsActive,
				TotalRooms = rooms.Count()
			};
		}
	}
}
