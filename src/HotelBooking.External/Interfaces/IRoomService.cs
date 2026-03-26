using HotelBooking.EndPoint.Common.Dto;
using HotelBooking.EndPoint.Common.Enums;

namespace HotelBooking.External.Interfaces;

public interface IRoomService
{
	Task<IEnumerable<RoomDto>> GetAllRoomsAsync();
	Task<RoomDto?> GetRoomByIdAsync(Guid id);
	Task<IEnumerable<RoomDto>> GetRoomsByHotelAsync(Guid hotelId);
	Task<IEnumerable<RoomDto>> GetRoomsByStatusAsync(RoomStatus status);
	Task<RoomDto> CreateRoomAsync(CreateRoomDto createRoomDto);
	Task<RoomDto> UpdateRoomAsync(Guid id, CreateRoomDto updateRoomDto);
	Task<bool> UpdateRoomStatusAsync(Guid id, RoomStatus status);
	Task<bool> DeleteRoomAsync(Guid id);
	Task<RoomHistoryDto> GetRoomHistoryAsync(Guid roomId, DateOnly? startDate = null, DateOnly? endDate = null);
	Task<IEnumerable<RoomDto>> GetRoomsByCategoryAsync(RoomCategory category);
	Task<IEnumerable<AvailableRoomDto>> SearchAvailableRoomsAsync(RoomAvailabilitySearchDto searchDto);
	Task<bool> IsRoomAvailableAsync(Guid roomId, DateOnly checkIn, DateOnly checkOut);
}
