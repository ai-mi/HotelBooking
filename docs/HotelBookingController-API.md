# Hotel Booking Controller API Documentation

## Overview
The `HotelBookingController` provides RESTful API endpoints for managing hotel bookings.

**Base Route:** `/api/HotelBooking`

## Endpoints

### 1. Get All Bookings
**GET** `/api/HotelBooking`

Returns a list of all bookings in the system.

**Response:** `200 OK` with array of `BookingDto`

---

### 2. Get Booking by ID
**GET** `/api/HotelBooking/{id}`

Retrieves a specific booking by its unique identifier.

**Parameters:**
- `id` (Guid) - The booking ID

**Responses:**
- `200 OK` - Booking found
- `404 Not Found` - Booking not found

---

### 3. Get Booking by Reference
**GET** `/api/HotelBooking/reference/{reference}`

Retrieves a booking using its reference number.

**Parameters:**
- `reference` (string) - The booking reference number

**Responses:**
- `200 OK` - Booking found
- `404 Not Found` - Booking not found

---

### 4. Get Bookings by Customer
**GET** `/api/HotelBooking/customer/{customerId}`

Returns all bookings for a specific customer.

**Parameters:**
- `customerId` (Guid) - The customer ID

**Response:** `200 OK` with array of `BookingDto`

---

### 5. Get Bookings by Room
**GET** `/api/HotelBooking/room/{roomId}`

Returns all bookings for a specific room.

**Parameters:**
- `roomId` (Guid) - The room ID

**Response:** `200 OK` with array of `BookingDto`

---

### 6. Get Bookings by Date Range
**GET** `/api/HotelBooking/daterange?startDate={startDate}&endDate={endDate}`

Returns all bookings within a specified date range.

**Query Parameters:**
- `startDate` (DateOnly) - Start date (format: YYYY-MM-DD)
- `endDate` (DateOnly) - End date (format: YYYY-MM-DD)

**Responses:**
- `200 OK` - Bookings found
- `400 Bad Request` - Invalid date range

---

### 7. Create Booking
**POST** `/api/HotelBooking`

Creates a new booking.

**Request Body:** `CreateBookingDto`
```json
{
  "customerId": "guid",
  "roomId": "guid",
  "roomCategory": 1,
  "hotelId": "guid",
  "checkInDate": "2024-01-01",
  "checkOutDate": "2024-01-05",
  "numberOfGuests": 2,
  "source": 1,
  "specialRequests": "string"
}
```

**Validations:**
- Check-in date cannot be in the past
- Check-out date must be after check-in date
- Number of guests must be at least 1

**Responses:**
- `201 Created` - Booking created successfully
- `400 Bad Request` - Invalid input
- `409 Conflict` - Room not available for selected dates

---

### 8. Update Booking
**PUT** `/api/HotelBooking/{id}`

Updates an existing booking.

**Parameters:**
- `id` (Guid) - The booking ID

**Request Body:** `CreateBookingDto`

**Validations:**
- Check-in date cannot be in the past
- Check-out date must be after check-in date
- Number of guests must be at least 1

**Responses:**
- `200 OK` - Booking updated successfully
- `400 Bad Request` - Invalid input or cannot update cancelled booking
- `404 Not Found` - Booking not found

---

### 9. Cancel Booking
**POST** `/api/HotelBooking/{id}/cancel`

Cancels an existing booking.

**Parameters:**
- `id` (Guid) - The booking ID

**Request Body:**
```json
{
  "reason": "string"
}
```

**Responses:**
- `200 OK` - Booking cancelled successfully
- `404 Not Found` - Booking not found or already cancelled

---

### 10. Check In
**POST** `/api/HotelBooking/{id}/checkin`

Checks in a guest for their booking.

**Parameters:**
- `id` (Guid) - The booking ID

**Responses:**
- `200 OK` - Check-in successful
- `400 Bad Request` - Booking not in confirmed status
- `404 Not Found` - Booking not found

---

### 11. Check Out
**POST** `/api/HotelBooking/{id}/checkout`

Checks out a guest from their booking.

**Parameters:**
- `id` (Guid) - The booking ID

**Responses:**
- `200 OK` - Check-out successful
- `400 Bad Request` - Booking not in checked-in status
- `404 Not Found` - Booking not found

---

### 12. Calculate Price
**POST** `/api/HotelBooking/calculate-price`

Calculates the total price for a potential booking.

**Request Body:**
```json
{
  "roomId": "guid",
  "checkInDate": "2024-01-01",
  "checkOutDate": "2024-01-05",
  "customerId": "guid (optional)"
}
```

**Validations:**
- Check-out date must be after check-in date

**Response:** `200 OK`
```json
{
  "totalPrice": 500.00
}
```

---

## Data Models

### BookingDto
```json
{
  "id": "guid",
  "bookingReference": "string",
  "customerId": "guid",
  "customerName": "string",
  "roomId": "guid",
  "roomNumber": "string",
  "roomCategory": 1,
  "checkInDate": "2024-01-01",
  "checkOutDate": "2024-01-05",
  "numberOfGuests": 2,
  "status": 1,
  "source": 1,
  "totalPrice": 500.00,
  "discountAmount": 50.00,
  "specialRequests": "string",
  "createdAt": "2024-01-01T00:00:00Z"
}
```

### BookingStatus Enum
- `Confirmed` = 1
- `CheckedIn` = 2
- `CheckedOut` = 3
- `Cancelled` = 4

### BookingSource Enum
- `Website` = 1
- `Mobile` = 2
- `Phone` = 3
- `WalkIn` = 4

## Error Responses

All endpoints return standardized error responses:

**400 Bad Request:**
```json
{
  "message": "Error description"
}
```

**404 Not Found:**
```json
{
  "message": "Resource not found"
}
```

**409 Conflict:**
```json
{
  "message": "Conflict description"
}
```

**500 Internal Server Error:**
```json
"Error message string"
```

## Notes

- All dates use `DateOnly` type (format: YYYY-MM-DD)
- All GUIDs should be in standard format
- Prices are in decimal format with 2 decimal places
- All endpoints use JSON for request and response bodies
- Authentication/Authorization should be added as needed
