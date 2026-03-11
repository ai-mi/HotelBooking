using HotelBooking.EndPoint.Common.Dto;
using HotelBooking.EndPoint.Common.Enums;

namespace HotelBooking.External.Interfaces;

public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetRoomsByCategoryAsync(RoomCategory category);
    Task<IEnumerable<AvailableRoomDto>> SearchAvailableRoomsAsync(RoomAvailabilitySearchDto searchDto);
	Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime checkIn, DateTime checkOut);	
}
