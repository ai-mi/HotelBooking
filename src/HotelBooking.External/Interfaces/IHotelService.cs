using HotelBooking.EndPoint.Common.Dto;

namespace HotelBooking.External.Interfaces
{
	public interface IHotelService
	{
		Task<HotelDto?> GetHotelByIdAsync(Guid id);
		Task<IEnumerable<HotelDto>> GetAllHotelsAsync();
		Task<IEnumerable<HotelDto>> GetActiveHotelsAsync();
		Task<IEnumerable<HotelDto>> GetHotelsByCityAsync(string city);
		Task<IEnumerable<HotelDto>> GetHotelsByCountryAsync(string country);
		Task<HotelDto> CreateHotelAsync(CreateHotelDto createHotelDto);
		Task<HotelDto> UpdateHotelAsync(Guid id, CreateHotelDto updateHotelDto);
		Task<bool> DeactivateHotelAsync(Guid id);
		Task<bool> ActivateHotelAsync(Guid id);
		Task<IEnumerable<RoomDto>> GetHotelRoomsAsync(Guid hotelId);
	}
}
