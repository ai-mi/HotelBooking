using HotelBooking.EndPoint.Common.Dto;
using HotelBooking.EndPoint.Common.Enums;
using HotelBooking.External.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.EndPoint.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Produces("application/json")]
	public class RoomsController : ControllerBase
	{
		private readonly IRoomService _roomService;
		private readonly ILogger<RoomsController> _logger;

		public RoomsController(IRoomService roomService, ILogger<RoomsController> logger)
		{
			_roomService = roomService;
			_logger = logger;
		}


		/// <summary>
		/// Get all rooms
		/// </summary>
		/// <returns>List of all rooms</returns>
		[HttpGet]
		[ProducesResponseType(typeof(IEnumerable<RoomDto>), StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<RoomDto>>> GetAllRooms()
		{
			try
			{
				var rooms = await _roomService.GetAllRoomsAsync();
				return Ok(rooms);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving all rooms");
				return StatusCode(500, "An error occurred while retrieving rooms");
			}
		}

		/// <summary>
		/// Get a room by ID
		/// </summary>
		/// <param name="id">Room ID</param>
		/// <returns>Room details</returns>
		[HttpGet("{id}")]
		[ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<RoomDto>> GetRoomById(Guid id)
		{
			try
			{
				var room = await _roomService.GetRoomByIdAsync(id);

				if (room == null)
					return NotFound(new { message = $"Room with ID {id} not found" });

				return Ok(room);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving room {RoomId}", id);
				return StatusCode(500, "An error occurred while retrieving the room");
			}
		}


		/// <summary>
		/// Get all rooms in a specific hotel
		/// </summary>
		/// <param name="hotelId">Hotel ID</param>
		/// <returns>List of rooms in the hotel</returns>
		[HttpGet("hotel/{hotelId}")]
		[ProducesResponseType(typeof(IEnumerable<RoomDto>), StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<RoomDto>>> GetRoomsByHotel(Guid hotelId)
		{
			try
			{
				var rooms = await _roomService.GetRoomsByHotelAsync(hotelId);
				return Ok(rooms);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving rooms for hotel {HotelId}", hotelId);
				return StatusCode(500, "An error occurred while retrieving rooms");
			}
		}


		/// <summary>
		/// Get rooms by category
		/// </summary>
		/// <param name="category">Room category (Single=1, Double=2, Twin=3, Suite=4, Deluxe=5, Presidential=6)</param>
		/// <returns>List of rooms in the specified category</returns>
		[HttpGet("category/{category}")]
		[ProducesResponseType(typeof(IEnumerable<RoomDto>), StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<RoomDto>>> GetRoomsByCategory(RoomCategory category)
		{
			try
			{
				var rooms = await _roomService.GetRoomsByCategoryAsync(category);
				return Ok(rooms);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving rooms for category {Category}", category);
				return StatusCode(500, "An error occurred while retrieving rooms");
			}
		}

		/// <summary>
		/// Get rooms by status
		/// </summary>
		/// <param name="status">Room status (Available=1, Occupied=2, Maintenance=3, Cleaning=4, OutOfService=5)</param>
		/// <returns>List of rooms with the specified status</returns>
		[HttpGet("status/{status}")]
		[ProducesResponseType(typeof(IEnumerable<RoomDto>), StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<RoomDto>>> GetRoomsByStatus(RoomStatus status)
		{
			try
			{
				var rooms = await _roomService.GetRoomsByStatusAsync(status);
				return Ok(rooms);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving rooms with status {Status}", status);
				return StatusCode(500, "An error occurred while retrieving rooms");
			}
		}


		/// <summary>
		/// Search for available rooms based on criteria
		/// </summary>
		/// <param name="searchDto">Search criteria including dates, hotel, category, and number of guests</param>
		/// <returns>List of available rooms matching the criteria</returns>
		[HttpPost("search")]
		[ProducesResponseType(typeof(IEnumerable<AvailableRoomDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<IEnumerable<AvailableRoomDto>>> SearchAvailableRooms([FromBody] RoomAvailabilitySearchDto searchDto)
		{
			try
			{
				if (searchDto.CheckInDate < DateOnly.FromDateTime(DateTime.UtcNow.Date))
					return BadRequest(new { message = "Check-in date cannot be in the past" });

				if (searchDto.CheckOutDate <= searchDto.CheckInDate)
					return BadRequest(new { message = "Check-out date must be after check-in date" });

				if (searchDto.NumberOfGuests < 1)
					return BadRequest(new { message = "Number of guests must be at least 1" });

				var availableRooms = await _roomService.SearchAvailableRoomsAsync(searchDto);
				return Ok(availableRooms);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error searching for available rooms");
				return StatusCode(500, "An error occurred while searching for rooms");
			}
		}


		/// <summary>
		/// Create a new room
		/// </summary>
		/// <param name="createRoomDto">Room creation data</param>
		/// <returns>Created room details</returns>
		[HttpPost]
		[ProducesResponseType(typeof(RoomDto), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<RoomDto>> CreateRoom([FromBody] CreateRoomDto createRoomDto)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(createRoomDto.RoomNumber))
					return BadRequest(new { message = "Room number is required" });

				if (createRoomDto.PricePerNight <= 0)
					return BadRequest(new { message = "Price per night must be greater than 0" });

				if (createRoomDto.MaxOccupancy < 1)
					return BadRequest(new { message = "Max occupancy must be at least 1" });

				var room = await _roomService.CreateRoomAsync(createRoomDto);
				return CreatedAtAction(nameof(GetRoomById), new { id = room.Id }, room);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating room");
				return StatusCode(500, "An error occurred while creating the room");
			}
		}

		/// <summary>
		/// Update an existing room
		/// </summary>
		/// <param name="id">Room ID</param>
		/// <param name="updateRoomDto">Updated room data</param>
		/// <returns>Updated room details</returns>
		[HttpPut("{id}")]
		[ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<RoomDto>> UpdateRoom(Guid id, [FromBody] CreateRoomDto updateRoomDto)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(updateRoomDto.RoomNumber))
					return BadRequest(new { message = "Room number is required" });

				if (updateRoomDto.PricePerNight <= 0)
					return BadRequest(new { message = "Price per night must be greater than 0" });

				if (updateRoomDto.MaxOccupancy < 1)
					return BadRequest(new { message = "Max occupancy must be at least 1" });

				var room = await _roomService.UpdateRoomAsync(id, updateRoomDto);
				return Ok(room);
			}
			catch (ArgumentException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating room {RoomId}", id);
				return StatusCode(500, "An error occurred while updating the room");
			}
		}

		/// <summary>
		/// Update room status
		/// </summary>
		/// <param name="id">Room ID</param>
		/// <param name="status">New room status (Available=1, Occupied=2, Maintenance=3, Cleaning=4, OutOfService=5)</param>
		/// <returns>Success message</returns>
		[HttpPatch("{id}/status")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> UpdateRoomStatus(Guid id, [FromBody] RoomStatus status)
		{
			try
			{
				var result = await _roomService.UpdateRoomStatusAsync(id, status);

				if (!result)
					return NotFound(new { message = $"Room with ID {id} not found" });

				return Ok(new { message = $"Room status updated to {status}" });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating room status for {RoomId}", id);
				return StatusCode(500, "An error occurred while updating room status");
			}
		}

		/// <summary>
		/// Delete a room
		/// </summary>
		/// <param name="id">Room ID</param>
		/// <returns>Success message</returns>
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<ActionResult> DeleteRoom(Guid id)
		{
			try
			{
				var result = await _roomService.DeleteRoomAsync(id);

				if (!result)
					return NotFound(new { message = $"Room with ID {id} not found" });

				return Ok(new { message = "Room deleted successfully" });
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting room {RoomId}", id);
				return StatusCode(500, "An error occurred while deleting the room");
			}
		}

		/// <summary>
		/// Get room history with bookings and audit logs
		/// </summary>
		/// <param name="id">Room ID</param>
		/// <param name="startDate">Optional start date filter</param>
		/// <param name="endDate">Optional end date filter</param>
		/// <returns>Room history with statistics</returns>
		[HttpGet("{id}/history")]
		[ProducesResponseType(typeof(RoomHistoryDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<RoomHistoryDto>> GetRoomHistory(
			Guid id,
			[FromQuery] DateOnly? startDate = null,
			[FromQuery] DateOnly? endDate = null)
		{
			try
			{
				var history = await _roomService.GetRoomHistoryAsync(id, startDate, endDate);
				return Ok(history);
			}
			catch (ArgumentException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving room history for {RoomId}", id);
				return StatusCode(500, "An error occurred while retrieving room history");
			}
		}

		/// <summary>
		/// Check if a room is available for specific dates
		/// </summary>
		/// <param name="id">Room ID</param>
		/// <param name="checkIn">Check-in date</param>
		/// <param name="checkOut">Check-out date</param>
		/// <returns>Availability status</returns>
		[HttpGet("{id}/availability")]
		[ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult> CheckRoomAvailability(
			Guid id,
			[FromQuery] DateOnly checkIn,
			[FromQuery] DateOnly checkOut)
		{
			try
			{
				if (checkIn < DateOnly.FromDateTime(DateTime.UtcNow.Date))
					return BadRequest(new { message = "Check-in date cannot be in the past" });

				if (checkOut <= checkIn)
					return BadRequest(new { message = "Check-out date must be after check-in date" });

				var isAvailable = await _roomService.IsRoomAvailableAsync(id, checkIn, checkOut);

				return Ok(new
				{
					roomId = id,
					checkIn = checkIn,
					checkOut = checkOut,
					isAvailable = isAvailable,
					message = isAvailable ? "Room is available for the selected dates" : "Room is not available for the selected dates"
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error checking room availability for {RoomId}", id);
				return StatusCode(500, "An error occurred while checking room availability");
			}
		}
	}

}
