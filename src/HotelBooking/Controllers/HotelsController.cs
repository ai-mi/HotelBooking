using HotelBooking.EndPoint.Common.Dto;
using HotelBooking.External.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.EndPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly ILogger<HotelsController> _logger;

        public HotelsController(IHotelService hotelService, ILogger<HotelsController> logger)
        {
            _hotelService = hotelService;
            _logger = logger;
        }

        /// <summary>
        /// Get all hotels
        /// </summary>
        /// <returns>List of all hotels</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<HotelDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetAllHotels()
        {
            try
            {
                var hotels = await _hotelService.GetAllHotelsAsync();
                return Ok(hotels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all hotels");
                return StatusCode(500, "An error occurred while retrieving hotels");
            }
        }

        /// <summary>
        /// Get all active hotels
        /// </summary>
        /// <returns>List of active hotels</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<HotelDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetActiveHotels()
        {
            try
            {
                var hotels = await _hotelService.GetActiveHotelsAsync();
                return Ok(hotels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active hotels");
                return StatusCode(500, "An error occurred while retrieving hotels");
            }
        }

        /// <summary>
        /// Get a hotel by ID
        /// </summary>
        /// <param name="id">Hotel ID</param>
        /// <returns>Hotel details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(HotelDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HotelDto>> GetHotelById(Guid id)
        {
            try
            {
                var hotel = await _hotelService.GetHotelByIdAsync(id);
                if (hotel == null)
                    return NotFound(new { message = "Hotel not found" });

                return Ok(hotel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving hotel {HotelId}", id);
                return StatusCode(500, "An error occurred while retrieving the hotel");
            }
        }

        /// <summary>
        /// Get hotels by city
        /// </summary>
        /// <param name="city">City name</param>
        /// <returns>List of hotels in the specified city</returns>
        [HttpGet("city/{city}")]
        [ProducesResponseType(typeof(IEnumerable<HotelDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotelsByCity(string city)
        {
            try
            {
                var hotels = await _hotelService.GetHotelsByCityAsync(city);
                return Ok(hotels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving hotels in city {City}", city);
                return StatusCode(500, "An error occurred while retrieving hotels");
            }
        }

        /// <summary>
        /// Get hotels by country
        /// </summary>
        /// <param name="country">Country name</param>
        /// <returns>List of hotels in the specified country</returns>
        [HttpGet("country/{country}")]
        [ProducesResponseType(typeof(IEnumerable<HotelDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotelsByCountry(string country)
        {
            try
            {
                var hotels = await _hotelService.GetHotelsByCountryAsync(country);
                return Ok(hotels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving hotels in country {Country}", country);
                return StatusCode(500, "An error occurred while retrieving hotels");
            }
        }

        /// <summary>
        /// Get all rooms for a specific hotel
        /// </summary>
        /// <param name="id">Hotel ID</param>
        /// <returns>List of rooms in the hotel</returns>
        [HttpGet("{id}/rooms")]
        [ProducesResponseType(typeof(IEnumerable<RoomDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetHotelRooms(Guid id)
        {
            try
            {
                var rooms = await _hotelService.GetHotelRoomsAsync(id);
                return Ok(rooms);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Hotel not found {HotelId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rooms for hotel {HotelId}", id);
                return StatusCode(500, "An error occurred while retrieving hotel rooms");
            }
        }

        /// <summary>
        /// Create a new hotel
        /// </summary>
        /// <param name="createHotelDto">Hotel details</param>
        /// <returns>Created hotel</returns>
        [HttpPost]
        [ProducesResponseType(typeof(HotelDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<HotelDto>> CreateHotel([FromBody] CreateHotelDto createHotelDto)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(createHotelDto.Name))
                    return BadRequest(new { message = "Hotel name is required" });

                if (string.IsNullOrWhiteSpace(createHotelDto.Address))
                    return BadRequest(new { message = "Address is required" });

                if (string.IsNullOrWhiteSpace(createHotelDto.City))
                    return BadRequest(new { message = "City is required" });

                if (string.IsNullOrWhiteSpace(createHotelDto.Country))
                    return BadRequest(new { message = "Country is required" });

                var hotel = await _hotelService.CreateHotelAsync(createHotelDto);
                return CreatedAtAction(nameof(GetHotelById), new { id = hotel.Id }, hotel);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid hotel creation request");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating hotel");
                return StatusCode(500, "An error occurred while creating the hotel");
            }
        }

        /// <summary>
        /// Update an existing hotel
        /// </summary>
        /// <param name="id">Hotel ID</param>
        /// <param name="updateHotelDto">Updated hotel details</param>
        /// <returns>Updated hotel</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(HotelDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HotelDto>> UpdateHotel(Guid id, [FromBody] CreateHotelDto updateHotelDto)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(updateHotelDto.Name))
                    return BadRequest(new { message = "Hotel name is required" });

                if (string.IsNullOrWhiteSpace(updateHotelDto.Address))
                    return BadRequest(new { message = "Address is required" });

                if (string.IsNullOrWhiteSpace(updateHotelDto.City))
                    return BadRequest(new { message = "City is required" });

                if (string.IsNullOrWhiteSpace(updateHotelDto.Country))
                    return BadRequest(new { message = "Country is required" });

                var hotel = await _hotelService.UpdateHotelAsync(id, updateHotelDto);
                return Ok(hotel);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid update request for hotel {HotelId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating hotel {HotelId}", id);
                return StatusCode(500, "An error occurred while updating the hotel");
            }
        }

        /// <summary>
        /// Deactivate a hotel
        /// </summary>
        /// <param name="id">Hotel ID</param>
        /// <returns>Success status</returns>
        [HttpPost("{id}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeactivateHotel(Guid id)
        {
            try
            {
                var result = await _hotelService.DeactivateHotelAsync(id);
                if (!result)
                    return NotFound(new { message = "Hotel not found" });

                return Ok(new { message = "Hotel deactivated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating hotel {HotelId}", id);
                return StatusCode(500, "An error occurred while deactivating the hotel");
            }
        }

        /// <summary>
        /// Activate a hotel
        /// </summary>
        /// <param name="id">Hotel ID</param>
        /// <returns>Success status</returns>
        [HttpPost("{id}/activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ActivateHotel(Guid id)
        {
            try
            {
                var result = await _hotelService.ActivateHotelAsync(id);
                if (!result)
                    return NotFound(new { message = "Hotel not found" });

                return Ok(new { message = "Hotel activated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating hotel {HotelId}", id);
                return StatusCode(500, "An error occurred while activating the hotel");
            }
        }
    }
}
