# Hotels Controller API Documentation

## Overview
The `HotelsController` provides RESTful API endpoints for managing hotels in the system.

**Base Route:** `/api/Hotels`

## Endpoints

### 1. Get All Hotels
**GET** `/api/Hotels`

Returns a list of all hotels in the system (both active and inactive).

**Response:** `200 OK` with array of `HotelDto`

**Example Response:**
```json
[
  {
    "id": "guid",
    "name": "Grand Hotel",
    "address": "123 Main Street",
    "city": "New York",
    "country": "USA",
    "phoneNumber": "+1-555-0100",
    "email": "contact@grandhotel.com",
    "isActive": true,
    "totalRooms": 150
  }
]
```

---

### 2. Get Active Hotels
**GET** `/api/Hotels/active`

Returns a list of only active hotels.

**Response:** `200 OK` with array of `HotelDto`

---

### 3. Get Hotel by ID
**GET** `/api/Hotels/{id}`

Retrieves a specific hotel by its unique identifier.

**Parameters:**
- `id` (Guid) - The hotel ID

**Responses:**
- `200 OK` - Hotel found
- `404 Not Found` - Hotel not found

**Example Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Grand Hotel",
  "address": "123 Main Street",
  "city": "New York",
  "country": "USA",
  "phoneNumber": "+1-555-0100",
  "email": "contact@grandhotel.com",
  "isActive": true,
  "totalRooms": 150
}
```

---

### 4. Get Hotels by City
**GET** `/api/Hotels/city/{city}`

Returns all hotels in a specific city.

**Parameters:**
- `city` (string) - The city name

**Response:** `200 OK` with array of `HotelDto`

**Example:** `/api/Hotels/city/New York`

---

### 5. Get Hotels by Country
**GET** `/api/Hotels/country/{country}`

Returns all hotels in a specific country.

**Parameters:**
- `country` (string) - The country name

**Response:** `200 OK` with array of `HotelDto`

**Example:** `/api/Hotels/country/USA`

---

### 6. Get Hotel Rooms
**GET** `/api/Hotels/{id}/rooms`

Returns all rooms for a specific hotel.

**Parameters:**
- `id` (Guid) - The hotel ID

**Responses:**
- `200 OK` - Returns array of `RoomDto`
- `404 Not Found` - Hotel not found

**Example Response:**
```json
[
  {
    "id": "guid",
    "roomNumber": "101",
    "hotelId": "guid",
    "hotelName": "Grand Hotel",
    "category": 1,
    "pricePerNight": 150.00,
    "floor": 1,
    "maxOccupancy": 2,
    "status": 1,
    "description": "Standard room with city view"
  }
]
```

---

### 7. Create Hotel
**POST** `/api/Hotels`

Creates a new hotel.

**Request Body:** `CreateHotelDto`
```json
{
  "name": "Grand Hotel",
  "address": "123 Main Street",
  "city": "New York",
  "country": "USA",
  "phoneNumber": "+1-555-0100",
  "email": "contact@grandhotel.com"
}
```

**Validations:**
- Hotel name is required
- Address is required
- City is required
- Country is required

**Responses:**
- `201 Created` - Hotel created successfully
- `400 Bad Request` - Invalid input

**Example Success Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Grand Hotel",
  "address": "123 Main Street",
  "city": "New York",
  "country": "USA",
  "phoneNumber": "+1-555-0100",
  "email": "contact@grandhotel.com",
  "isActive": true,
  "totalRooms": 0
}
```

---

### 8. Update Hotel
**PUT** `/api/Hotels/{id}`

Updates an existing hotel.

**Parameters:**
- `id` (Guid) - The hotel ID

**Request Body:** `CreateHotelDto`
```json
{
  "name": "Grand Hotel & Spa",
  "address": "123 Main Street",
  "city": "New York",
  "country": "USA",
  "phoneNumber": "+1-555-0100",
  "email": "contact@grandhotel.com"
}
```

**Validations:**
- Hotel name is required
- Address is required
- City is required
- Country is required

**Responses:**
- `200 OK` - Hotel updated successfully
- `400 Bad Request` - Invalid input
- `404 Not Found` - Hotel not found

---

### 9. Deactivate Hotel
**POST** `/api/Hotels/{id}/deactivate`

Deactivates a hotel (soft delete - sets IsActive to false).

**Parameters:**
- `id` (Guid) - The hotel ID

**Responses:**
- `200 OK` - Hotel deactivated successfully
- `404 Not Found` - Hotel not found

**Example Success Response:**
```json
{
  "message": "Hotel deactivated successfully"
}
```

---

### 10. Activate Hotel
**POST** `/api/Hotels/{id}/activate`

Activates a previously deactivated hotel.

**Parameters:**
- `id` (Guid) - The hotel ID

**Responses:**
- `200 OK` - Hotel activated successfully
- `404 Not Found` - Hotel not found

**Example Success Response:**
```json
{
  "message": "Hotel activated successfully"
}
```

---

## Data Models

### HotelDto
```json
{
  "id": "guid",
  "name": "string",
  "address": "string",
  "city": "string",
  "country": "string",
  "phoneNumber": "string",
  "email": "string",
  "isActive": boolean,
  "totalRooms": integer
}
```

### CreateHotelDto
```json
{
  "name": "string (required)",
  "address": "string (required)",
  "city": "string (required)",
  "country": "string (required)",
  "phoneNumber": "string",
  "email": "string"
}
```

### RoomDto
See [RoomsController API Documentation](./RoomsController-API.md) for details.

---

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

**500 Internal Server Error:**
```json
"Error message string"
```

---

## Business Rules

1. **Hotel Creation:**
   - New hotels are active by default (IsActive = true)
   - TotalRooms is calculated automatically based on rooms added to the hotel

2. **Hotel Deactivation:**
   - Uses soft delete (IsActive flag)
   - Does not physically delete the hotel or its rooms
   - Deactivated hotels may not accept new bookings

3. **Hotel Search:**
   - City and country searches are case-sensitive
   - Use the "active" endpoint to get only active hotels

4. **Required Fields:**
   - Name, Address, City, and Country are mandatory
   - Phone number and email are optional but recommended

---

## Usage Examples

### Search for hotels in a specific city
```http
GET /api/Hotels/city/New York
```

### Create a new hotel
```http
POST /api/Hotels
Content-Type: application/json

{
  "name": "Luxury Resort",
  "address": "456 Beach Road",
  "city": "Miami",
  "country": "USA",
  "phoneNumber": "+1-555-0200",
  "email": "info@luxuryresort.com"
}
```

### Get all rooms in a hotel
```http
GET /api/Hotels/3fa85f64-5717-4562-b3fc-2c963f66afa6/rooms
```

### Deactivate a hotel
```http
POST /api/Hotels/3fa85f64-5717-4562-b3fc-2c963f66afa6/deactivate
```

---

## Notes

- All endpoints use JSON for request and response bodies
- All GUIDs should be in standard format
- Authentication/Authorization should be added as needed
- Consider implementing rate limiting for public endpoints
- The IsActive flag allows for soft deletion and easy restoration
