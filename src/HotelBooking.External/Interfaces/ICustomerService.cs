using HotelBooking.EndPoint.Common.Dto;

namespace HotelBooking.External.Interfaces
{
	public interface ICustomerService
	{
		Task<CustomerDto?> GetCustomerByIdAsync(Guid id);
		Task<CustomerDto?> GetCustomerByEmailAsync(string email);
		Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
		Task<IEnumerable<CustomerDto>> GetActiveCustomersAsync();
		Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto);
		Task<CustomerDto> UpdateCustomerAsync(Guid id, CreateCustomerDto updateCustomerDto);
		Task<bool> DeactivateCustomerAsync(Guid id);
		Task<bool> ActivateCustomerAsync(Guid id);
		Task<IEnumerable<BookingDto>> GetCustomerBookingsAsync(Guid customerId);
	}
}
