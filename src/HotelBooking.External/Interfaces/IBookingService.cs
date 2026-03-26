using HotelBooking.EndPoint.Common.Dto;

namespace HotelBooking.External.Interfaces
{
	public interface IBookingService
	{
		Task<BookingDto?> GetBookingByIdAsync(Guid id);
		Task<BookingDto?> GetBookingByReferenceAsync(string bookingReference);
		Task<IEnumerable<BookingDto>> GetAllBookingsAsync();
		Task<IEnumerable<BookingDto>> GetBookingsByCustomerAsync(Guid customerId);
		Task<IEnumerable<BookingDto>> GetBookingsByRoomAsync(Guid roomId);
		Task<IEnumerable<BookingDto>> GetBookingsByDateRangeAsync(DateOnly startDate, DateOnly endDate);
		Task<BookingDto> CreateBookingAsync(CreateBookingDto createBookingDto);
		Task<BookingDto> UpdateBookingAsync(Guid id, CreateBookingDto updateBookingDto);
		Task<bool> CancelBookingAsync(Guid id, string cancellationReason);
		Task<bool> CheckInBookingAsync(Guid id);
		Task<bool> CheckOutBookingAsync(Guid id);
		Task<decimal> CalculateTotalPriceAsync(Guid roomId, DateOnly checkIn, DateOnly checkOut, Guid? customerId = null);
	}
}
