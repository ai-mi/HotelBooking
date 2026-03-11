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
				if (searchDto.CheckInDate < DateTime.UtcNow.Date)
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
			[FromQuery] DateTime checkIn,
			[FromQuery] DateTime checkOut)
		{
			try
			{
				if (checkIn < DateTime.UtcNow.Date)
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
