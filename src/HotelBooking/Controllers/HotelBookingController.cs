using HotelBooking.EndPoint.Common.Dto;
using HotelBooking.External.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.EndPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class HotelBookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<HotelBookingController> _logger;

        public HotelBookingController(IBookingService bookingService, ILogger<HotelBookingController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        /// <summary>
        /// Get all bookings
        /// </summary>
        /// <returns>List of all bookings</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetAllBookings()
        {
            try
            {
                var bookings = await _bookingService.GetAllBookingsAsync();
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all bookings");
                return StatusCode(500, "An error occurred while retrieving bookings");
            }
        }

        /// <summary>
        /// Get a booking by ID
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Booking details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookingDto>> GetBookingById(Guid id)
        {
            try
            {
                var booking = await _bookingService.GetBookingByIdAsync(id);
                if (booking == null)
                    return NotFound(new { message = "Booking not found" });

                return Ok(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking {BookingId}", id);
                return StatusCode(500, "An error occurred while retrieving the booking");
            }
        }

        /// <summary>
        /// Get a booking by reference number
        /// </summary>
        /// <param name="reference">Booking reference number</param>
        /// <returns>Booking details</returns>
        [HttpGet("reference/{reference}")]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookingDto>> GetBookingByReference(string reference)
        {
            try
            {
                var booking = await _bookingService.GetBookingByReferenceAsync(reference);
                if (booking == null)
                    return NotFound(new { message = "Booking not found" });

                return Ok(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking with reference {Reference}", reference);
                return StatusCode(500, "An error occurred while retrieving the booking");
            }
        }

        /// <summary>
        /// Get all bookings for a customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>List of bookings for the customer</returns>
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByCustomer(Guid customerId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByCustomerAsync(customerId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings for customer {CustomerId}", customerId);
                return StatusCode(500, "An error occurred while retrieving bookings");
            }
        }

        /// <summary>
        /// Get all bookings for a room
        /// </summary>
        /// <param name="roomId">Room ID</param>
        /// <returns>List of bookings for the room</returns>
        [HttpGet("room/{roomId}")]
        [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByRoom(Guid roomId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByRoomAsync(roomId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings for room {RoomId}", roomId);
                return StatusCode(500, "An error occurred while retrieving bookings");
            }
        }

        /// <summary>
        /// Get bookings within a date range
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>List of bookings within the date range</returns>
        [HttpGet("daterange")]
        [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByDateRange(
            [FromQuery] DateOnly startDate,
            [FromQuery] DateOnly endDate)
        {
            try
            {
                if (endDate < startDate)
                    return BadRequest(new { message = "End date must be after start date" });

                var bookings = await _bookingService.GetBookingsByDateRangeAsync(startDate, endDate);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings for date range {StartDate} to {EndDate}", startDate, endDate);
                return StatusCode(500, "An error occurred while retrieving bookings");
            }
        }

        /// <summary>
        /// Create a new booking
        /// </summary>
        /// <param name="createBookingDto">Booking details</param>
        /// <returns>Created booking</returns>
        [HttpPost]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] CreateBookingDto createBookingDto)
        {
            try
            {
                if (createBookingDto.CheckInDate < DateOnly.FromDateTime(DateTime.UtcNow.Date))
                    return BadRequest(new { message = "Check-in date cannot be in the past" });

                if (createBookingDto.CheckOutDate <= createBookingDto.CheckInDate)
                    return BadRequest(new { message = "Check-out date must be after check-in date" });

                if (createBookingDto.NumberOfGuests < 1)
                    return BadRequest(new { message = "Number of guests must be at least 1" });

                var booking = await _bookingService.CreateBookingAsync(createBookingDto);
                return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, booking);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid booking request");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Booking operation failed");
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                return StatusCode(500, "An error occurred while creating the booking");
            }
        }

        /// <summary>
        /// Update an existing booking
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="updateBookingDto">Updated booking details</param>
        /// <returns>Updated booking</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookingDto>> UpdateBooking(Guid id, [FromBody] CreateBookingDto updateBookingDto)
        {
            try
            {
                if (updateBookingDto.CheckInDate < DateOnly.FromDateTime(DateTime.UtcNow.Date))
                    return BadRequest(new { message = "Check-in date cannot be in the past" });

                if (updateBookingDto.CheckOutDate <= updateBookingDto.CheckInDate)
                    return BadRequest(new { message = "Check-out date must be after check-in date" });

                if (updateBookingDto.NumberOfGuests < 1)
                    return BadRequest(new { message = "Number of guests must be at least 1" });

                var booking = await _bookingService.UpdateBookingAsync(id, updateBookingDto);
                return Ok(booking);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid update request for booking {BookingId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Update operation failed for booking {BookingId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking {BookingId}", id);
                return StatusCode(500, "An error occurred while updating the booking");
            }
        }

        /// <summary>
        /// Cancel a booking
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="cancellationRequest">Cancellation details</param>
        /// <returns>Success status</returns>
        [HttpPost("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CancelBooking(Guid id, [FromBody] CancellationRequestDto cancellationRequest)
        {
            try
            {
                var result = await _bookingService.CancelBookingAsync(id, cancellationRequest.Reason ?? "No reason provided");
                if (!result)
                    return NotFound(new { message = "Booking not found or already cancelled" });

                return Ok(new { message = "Booking cancelled successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking {BookingId}", id);
                return StatusCode(500, "An error occurred while cancelling the booking");
            }
        }

        /// <summary>
        /// Check in a booking
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Success status</returns>
        [HttpPost("{id}/checkin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CheckIn(Guid id)
        {
            try
            {
                var result = await _bookingService.CheckInBookingAsync(id);
                if (!result)
                    return BadRequest(new { message = "Booking not found or not in confirmed status" });

                return Ok(new { message = "Check-in successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking in booking {BookingId}", id);
                return StatusCode(500, "An error occurred while checking in");
            }
        }

        /// <summary>
        /// Check out a booking
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Success status</returns>
        [HttpPost("{id}/checkout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CheckOut(Guid id)
        {
            try
            {
                var result = await _bookingService.CheckOutBookingAsync(id);
                if (!result)
                    return BadRequest(new { message = "Booking not found or not in checked-in status" });

                return Ok(new { message = "Check-out successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking out booking {BookingId}", id);
                return StatusCode(500, "An error occurred while checking out");
            }
        }

        /// <summary>
        /// Calculate total price for a booking
        /// </summary>
        /// <param name="request">Price calculation request</param>
        /// <returns>Total price</returns>
        [HttpPost("calculate-price")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<decimal>> CalculatePrice([FromBody] PriceCalculationRequestDto request)
        {
            try
            {
                if (request.CheckOutDate <= request.CheckInDate)
                    return BadRequest(new { message = "Check-out date must be after check-in date" });

                var price = await _bookingService.CalculateTotalPriceAsync(
                    request.RoomId, 
                    request.CheckInDate, 
                    request.CheckOutDate, 
                    request.CustomerId);

                return Ok(new { totalPrice = price });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid price calculation request");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating price");
                return StatusCode(500, "An error occurred while calculating the price");
            }
        }
    }

    // Helper DTOs for request bodies
    public class CancellationRequestDto
    {
        public string? Reason { get; set; }
    }

    public class PriceCalculationRequestDto
    {
        public Guid RoomId { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public Guid? CustomerId { get; set; }
    }
}

