using HotelBooking.EndPoint.Common.Dto;
using HotelBooking.External.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.EndPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        /// <summary>
        /// Get all customers
        /// </summary>
        /// <returns>List of all customers</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CustomerDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomers()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all customers");
                return StatusCode(500, "An error occurred while retrieving customers");
            }
        }

        /// <summary>
        /// Get all active customers
        /// </summary>
        /// <returns>List of active customers</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<CustomerDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetActiveCustomers()
        {
            try
            {
                var customers = await _customerService.GetActiveCustomersAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active customers");
                return StatusCode(500, "An error occurred while retrieving customers");
            }
        }

        /// <summary>
        /// Get a customer by ID
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>Customer details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDto>> GetCustomerById(Guid id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                    return NotFound(new { message = "Customer not found" });

                return Ok(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer {CustomerId}", id);
                return StatusCode(500, "An error occurred while retrieving the customer");
            }
        }

        /// <summary>
        /// Get a customer by email address
        /// </summary>
        /// <param name="email">Customer email address</param>
        /// <returns>Customer details</returns>
        [HttpGet("email/{email}")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDto>> GetCustomerByEmail(string email)
        {
            try
            {
                var customer = await _customerService.GetCustomerByEmailAsync(email);
                if (customer == null)
                    return NotFound(new { message = "Customer not found" });

                return Ok(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer with email {Email}", email);
                return StatusCode(500, "An error occurred while retrieving the customer");
            }
        }

        /// <summary>
        /// Get all bookings for a specific customer
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>List of bookings for the customer</returns>
        [HttpGet("{id}/bookings")]
        [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetCustomerBookings(Guid id)
        {
            try
            {
                var bookings = await _customerService.GetCustomerBookingsAsync(id);
                return Ok(bookings);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Customer not found {CustomerId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings for customer {CustomerId}", id);
                return StatusCode(500, "An error occurred while retrieving customer bookings");
            }
        }

        /// <summary>
        /// Create a new customer
        /// </summary>
        /// <param name="createCustomerDto">Customer details</param>
        /// <returns>Created customer</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CreateCustomerDto createCustomerDto)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(createCustomerDto.FirstName))
                    return BadRequest(new { message = "First name is required" });

                if (string.IsNullOrWhiteSpace(createCustomerDto.LastName))
                    return BadRequest(new { message = "Last name is required" });

                if (string.IsNullOrWhiteSpace(createCustomerDto.Email))
                    return BadRequest(new { message = "Email is required" });

                // Basic email validation
                if (!createCustomerDto.Email.Contains("@"))
                    return BadRequest(new { message = "Invalid email format" });

                if (string.IsNullOrWhiteSpace(createCustomerDto.PhoneNumber))
                    return BadRequest(new { message = "Phone number is required" });

                var customer = await _customerService.CreateCustomerAsync(createCustomerDto);
                return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid customer creation request");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Customer creation failed");
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                return StatusCode(500, "An error occurred while creating the customer");
            }
        }

        /// <summary>
        /// Update an existing customer
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <param name="updateCustomerDto">Updated customer details</param>
        /// <returns>Updated customer</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDto>> UpdateCustomer(Guid id, [FromBody] CreateCustomerDto updateCustomerDto)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(updateCustomerDto.FirstName))
                    return BadRequest(new { message = "First name is required" });

                if (string.IsNullOrWhiteSpace(updateCustomerDto.LastName))
                    return BadRequest(new { message = "Last name is required" });

                if (string.IsNullOrWhiteSpace(updateCustomerDto.Email))
                    return BadRequest(new { message = "Email is required" });

                // Basic email validation
                if (!updateCustomerDto.Email.Contains("@"))
                    return BadRequest(new { message = "Invalid email format" });

                if (string.IsNullOrWhiteSpace(updateCustomerDto.PhoneNumber))
                    return BadRequest(new { message = "Phone number is required" });

                var customer = await _customerService.UpdateCustomerAsync(id, updateCustomerDto);
                return Ok(customer);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid update request for customer {CustomerId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Customer update failed for {CustomerId}", id);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer {CustomerId}", id);
                return StatusCode(500, "An error occurred while updating the customer");
            }
        }

        /// <summary>
        /// Deactivate a customer account
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>Success status</returns>
        [HttpPost("{id}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeactivateCustomer(Guid id)
        {
            try
            {
                var result = await _customerService.DeactivateCustomerAsync(id);
                if (!result)
                    return NotFound(new { message = "Customer not found" });

                return Ok(new { message = "Customer account deactivated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating customer {CustomerId}", id);
                return StatusCode(500, "An error occurred while deactivating the customer account");
            }
        }

        /// <summary>
        /// Activate a customer account
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>Success status</returns>
        [HttpPost("{id}/activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ActivateCustomer(Guid id)
        {
            try
            {
                var result = await _customerService.ActivateCustomerAsync(id);
                if (!result)
                    return NotFound(new { message = "Customer not found" });

                return Ok(new { message = "Customer account activated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating customer {CustomerId}", id);
                return StatusCode(500, "An error occurred while activating the customer account");
            }
        }
    }
}
