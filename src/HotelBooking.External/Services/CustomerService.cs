using HotelBooking.EndPoint.Common.Dto;
using HotelBooking.EndPoint.Common.Enums;
using HotelBooking.EndPoint.Common.Interfaces;
using HotelBooking.EndPoint.Common.Models;
using HotelBooking.External.Interfaces;

namespace HotelBooking.External.Services
{
	public class CustomerService : ICustomerService
	{
		private readonly IUnitOfWork _unitOfWork;

		public CustomerService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<CustomerDto?> GetCustomerByIdAsync(Guid id)
		{
			var customer = await _unitOfWork.Customers.GetByIdAsync(id);
			if (customer == null)
				return null;

			return await MapToCustomerDtoAsync(customer);
		}

		public async Task<CustomerDto?> GetCustomerByEmailAsync(string email)
		{
			var customers = await _unitOfWork.Customers.FindAsync(c => c.Email == email);
			var customer = customers.FirstOrDefault();

			if (customer == null)
				return null;

			return await MapToCustomerDtoAsync(customer);
		}

		public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
		{
			var customers = await _unitOfWork.Customers.GetAllAsync();
			var customerDtos = new List<CustomerDto>();

			foreach (var customer in customers)
			{
				customerDtos.Add(await MapToCustomerDtoAsync(customer));
			}

			return customerDtos;
		}

		public async Task<IEnumerable<CustomerDto>> GetActiveCustomersAsync()
		{
			var customers = await _unitOfWork.Customers.FindAsync(c => c.IsActive);
			var customerDtos = new List<CustomerDto>();

			foreach (var customer in customers)
			{
				customerDtos.Add(await MapToCustomerDtoAsync(customer));
			}

			return customerDtos;
		}

		public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto)
		{
			// Check if email already exists
			var existingCustomer = await _unitOfWork.Customers.FindAsync(c => c.Email == createCustomerDto.Email);
			if (existingCustomer.Any())
				throw new InvalidOperationException("Email already exists");

			var customer = new Customer
			{
				Id = Guid.NewGuid(),
				FirstName = createCustomerDto.FirstName,
				LastName = createCustomerDto.LastName,
				Email = createCustomerDto.Email,
				PhoneNumber = createCustomerDto.PhoneNumber,
				Address = createCustomerDto.Address,
				City = createCustomerDto.City,
				Country = createCustomerDto.Country,
				DateOfBirth = createCustomerDto.DateOfBirth,
				PassportNumber = createCustomerDto.PassportNumber,
				IsActive = true,
				CreatedAt = DateTime.UtcNow
			};

			await _unitOfWork.Customers.AddAsync(customer);

			// Create audit log
			var auditLog = new CustomerAuditLog
			{
				Id = Guid.NewGuid(),
				CustomerId = customer.Id,
				Action = "Created",
				Details = "Customer account created",
				Timestamp = DateTime.UtcNow,
				CreatedAt = DateTime.UtcNow
			};
			await _unitOfWork.CustomerAuditLogs.AddAsync(auditLog);

			await _unitOfWork.SaveChangesAsync();

			return await MapToCustomerDtoAsync(customer);
		}

		public async Task<CustomerDto> UpdateCustomerAsync(Guid id, CreateCustomerDto updateCustomerDto)
		{
			var customer = await _unitOfWork.Customers.GetByIdAsync(id);
			if (customer == null)
				throw new ArgumentException("Customer not found");

			// Check if email is being changed and if it already exists
			if (customer.Email != updateCustomerDto.Email)
			{
				var existingCustomer = await _unitOfWork.Customers.FindAsync(c => c.Email == updateCustomerDto.Email);
				if (existingCustomer.Any())
					throw new InvalidOperationException("Email already exists");
			}

			customer.FirstName = updateCustomerDto.FirstName;
			customer.LastName = updateCustomerDto.LastName;
			customer.Email = updateCustomerDto.Email;
			customer.PhoneNumber = updateCustomerDto.PhoneNumber;
			customer.Address = updateCustomerDto.Address;
			customer.City = updateCustomerDto.City;
			customer.Country = updateCustomerDto.Country;
			customer.DateOfBirth = updateCustomerDto.DateOfBirth;
			customer.PassportNumber = updateCustomerDto.PassportNumber;
			customer.UpdatedAt = DateTime.UtcNow;

			await _unitOfWork.Customers.UpdateAsync(customer);

			// Create audit log
			var auditLog = new CustomerAuditLog
			{
				Id = Guid.NewGuid(),
				CustomerId = customer.Id,
				Action = "Updated",
				Details = "Customer information updated",
				Timestamp = DateTime.UtcNow,
				CreatedAt = DateTime.UtcNow
			};
			await _unitOfWork.CustomerAuditLogs.AddAsync(auditLog);

			await _unitOfWork.SaveChangesAsync();

			return await MapToCustomerDtoAsync(customer);
		}

		public async Task<bool> DeactivateCustomerAsync(Guid id)
		{
			var customer = await _unitOfWork.Customers.GetByIdAsync(id);
			if (customer == null)
				return false;

			customer.IsActive = false;
			customer.UpdatedAt = DateTime.UtcNow;

			await _unitOfWork.Customers.UpdateAsync(customer);

			var auditLog = new CustomerAuditLog
			{
				Id = Guid.NewGuid(),
				CustomerId = customer.Id,
				Action = "Deactivated",
				Details = "Customer account deactivated",
				Timestamp = DateTime.UtcNow,
				CreatedAt = DateTime.UtcNow
			};
			await _unitOfWork.CustomerAuditLogs.AddAsync(auditLog);

			await _unitOfWork.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ActivateCustomerAsync(Guid id)
		{
			var customer = await _unitOfWork.Customers.GetByIdAsync(id);
			if (customer == null)
				return false;

			customer.IsActive = true;
			customer.UpdatedAt = DateTime.UtcNow;

			await _unitOfWork.Customers.UpdateAsync(customer);

			var auditLog = new CustomerAuditLog
			{
				Id = Guid.NewGuid(),
				CustomerId = customer.Id,
				Action = "Activated",
				Details = "Customer account activated",
				Timestamp = DateTime.UtcNow,
				CreatedAt = DateTime.UtcNow
			};
			await _unitOfWork.CustomerAuditLogs.AddAsync(auditLog);

			await _unitOfWork.SaveChangesAsync();
			return true;
		}

		public async Task<IEnumerable<BookingDto>> GetCustomerBookingsAsync(Guid customerId)
		{
			var bookings = await _unitOfWork.Bookings.FindAsync(b => b.CustomerId == customerId);
			var bookingDtos = new List<BookingDto>();

			foreach (var booking in bookings.OrderByDescending(b => b.CheckInDate))
			{
				var room = await _unitOfWork.Rooms.GetByIdAsync(booking.RoomId);
				var customer = await _unitOfWork.Customers.GetByIdAsync(booking.CustomerId);

				bookingDtos.Add(new BookingDto
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
				});
			}

			return bookingDtos;
		}

		private async Task<CustomerDto> MapToCustomerDtoAsync(Customer customer)
		{
			var loyaltyMembers = await _unitOfWork.LoyaltyMembers.FindAsync(l => l.CustomerId == customer.Id);
			var loyaltyMember = loyaltyMembers.FirstOrDefault();

			return new CustomerDto
			{
				Id = customer.Id,
				FirstName = customer.FirstName,
				LastName = customer.LastName,
				Email = customer.Email,
				PhoneNumber = customer.PhoneNumber,
				IsLoyaltyMember = loyaltyMember != null,
				LoyaltyTier = loyaltyMember?.Tier.ToString(),
				LoyaltyPoints = loyaltyMember?.Points
			};
		}
	}
}
