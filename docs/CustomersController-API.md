# Customers Controller API Documentation

## Overview
The `CustomersController` provides RESTful API endpoints for managing customer accounts and retrieving customer-related information.

**Base Route:** `/api/Customers`

## Endpoints

### 1. Get All Customers
**GET** `/api/Customers`

Returns a list of all customers in the system (both active and inactive).

**Response:** `200 OK` with array of `CustomerDto`

**Example Response:**
```json
[
  {
    "id": "guid",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "+1-555-1234",
    "isLoyaltyMember": true,
    "loyaltyTier": "Gold",
    "loyaltyPoints": 1500
  }
]
```

---

### 2. Get Active Customers
**GET** `/api/Customers/active`

Returns a list of only active customer accounts.

**Response:** `200 OK` with array of `CustomerDto`

---

### 3. Get Customer by ID
**GET** `/api/Customers/{id}`

Retrieves a specific customer by their unique identifier.

**Parameters:**
- `id` (Guid) - The customer ID

**Responses:**
- `200 OK` - Customer found
- `404 Not Found` - Customer not found

**Example Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "+1-555-1234",
  "isLoyaltyMember": true,
  "loyaltyTier": "Gold",
  "loyaltyPoints": 1500
}
```

---

### 4. Get Customer by Email
**GET** `/api/Customers/email/{email}`

Retrieves a customer by their email address.

**Parameters:**
- `email` (string) - The customer's email address

**Responses:**
- `200 OK` - Customer found
- `404 Not Found` - Customer not found

**Example:** `/api/Customers/email/john.doe@example.com`

---

### 5. Get Customer Bookings
**GET** `/api/Customers/{id}/bookings`

Returns all bookings for a specific customer.

**Parameters:**
- `id` (Guid) - The customer ID

**Responses:**
- `200 OK` - Returns array of `BookingDto`
- `404 Not Found` - Customer not found

**Example Response:**
```json
[
  {
    "id": "guid",
    "bookingReference": "BK123456",
    "customerId": "guid",
    "customerName": "John Doe",
    "roomId": "guid",
    "roomNumber": "101",
    "roomCategory": 1,
    "checkInDate": "2024-01-01",
    "checkOutDate": "2024-01-05",
    "numberOfGuests": 2,
    "status": 1,
    "source": 1,
    "totalPrice": 600.00,
    "discountAmount": 60.00,
    "specialRequests": "Late check-out requested",
    "createdAt": "2023-12-15T10:30:00Z"
  }
]
```

---

### 6. Create Customer
**POST** `/api/Customers`

Creates a new customer account.

**Request Body:** `CreateCustomerDto`
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "+1-555-1234",
  "address": "123 Main Street",
  "city": "New York",
  "country": "USA",
  "dateOfBirth": "1990-01-15",
  "passportNumber": "AB123456"
}
```

**Validations:**
- First name is required
- Last name is required
- Email is required and must be valid format (contains @)
- Phone number is required

**Responses:**
- `201 Created` - Customer created successfully
- `400 Bad Request` - Invalid input
- `409 Conflict` - Email already exists

**Example Success Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "+1-555-1234",
  "isLoyaltyMember": false,
  "loyaltyTier": null,
  "loyaltyPoints": null
}
```

---

### 7. Update Customer
**PUT** `/api/Customers/{id}`

Updates an existing customer account.

**Parameters:**
- `id` (Guid) - The customer ID

**Request Body:** `CreateCustomerDto`
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@newemail.com",
  "phoneNumber": "+1-555-5678",
  "address": "456 Oak Avenue",
  "city": "Boston",
  "country": "USA",
  "dateOfBirth": "1990-01-15",
  "passportNumber": "AB123456"
}
```

**Validations:**
- First name is required
- Last name is required
- Email is required and must be valid format (contains @)
- Phone number is required

**Responses:**
- `200 OK` - Customer updated successfully
- `400 Bad Request` - Invalid input
- `404 Not Found` - Customer not found
- `409 Conflict` - Email already exists for another customer

---

### 8. Deactivate Customer
**POST** `/api/Customers/{id}/deactivate`

Deactivates a customer account (soft delete).

**Parameters:**
- `id` (Guid) - The customer ID

**Responses:**
- `200 OK` - Customer deactivated successfully
- `404 Not Found` - Customer not found

**Example Success Response:**
```json
{
  "message": "Customer account deactivated successfully"
}
```

**Note:** Deactivated customers:
- Cannot make new bookings
- Can still view their existing bookings
- Their data is preserved for reporting

---

### 9. Activate Customer
**POST** `/api/Customers/{id}/activate`

Activates a previously deactivated customer account.

**Parameters:**
- `id` (Guid) - The customer ID

**Responses:**
- `200 OK` - Customer activated successfully
- `404 Not Found` - Customer not found

**Example Success Response:**
```json
{
  "message": "Customer account activated successfully"
}
```

---

## Data Models

### CustomerDto
```json
{
  "id": "guid",
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "phoneNumber": "string",
  "isLoyaltyMember": boolean,
  "loyaltyTier": "string (nullable)",
  "loyaltyPoints": integer (nullable)
}
```

### CreateCustomerDto
```json
{
  "firstName": "string (required)",
  "lastName": "string (required)",
  "email": "string (required)",
  "phoneNumber": "string (required)",
  "address": "string (optional)",
  "city": "string (optional)",
  "country": "string (optional)",
  "dateOfBirth": "datetime (optional)",
  "passportNumber": "string (optional)"
}
```

### Loyalty Tiers
- `Bronze` - Entry level (0-999 points)
- `Silver` - Mid tier (1000-4999 points)
- `Gold` - High tier (5000-9999 points)
- `Platinum` - Premium tier (10000+ points)

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

**409 Conflict:**
```json
{
  "message": "Email already exists"
}
```

**500 Internal Server Error:**
```json
"Error message string"
```

---

## Business Rules

1. **Customer Creation:**
   - Email must be unique across all customers (active and inactive)
   - New customers are active by default
   - Loyalty membership is managed separately through the Loyalty Service

2. **Customer Deactivation:**
   - Uses soft delete (IsActive flag)
   - Does not physically delete customer data
   - Existing bookings remain accessible
   - Cannot create new bookings while deactivated

3. **Email Validation:**
   - Basic validation checks for @ symbol
   - More comprehensive email validation recommended at client level
   - Email is case-insensitive for lookups

4. **Customer Privacy:**
   - Sensitive information (passport, DOB) is not returned in CustomerDto
   - Personal data should be protected according to privacy regulations

5. **Loyalty Program:**
   - `isLoyaltyMember` indicates enrollment status
   - `loyaltyTier` and `loyaltyPoints` are managed by the Loyalty Service
   - Points are earned through bookings and other activities

---

## Usage Examples

### Register a new customer
```http
POST /api/Customers
Content-Type: application/json

{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@example.com",
  "phoneNumber": "+1-555-9876",
  "address": "789 Park Avenue",
  "city": "San Francisco",
  "country": "USA"
}
```

### Find customer by email
```http
GET /api/Customers/email/jane.smith@example.com
```

### Get all customer bookings
```http
GET /api/Customers/3fa85f64-5717-4562-b3fc-2c963f66afa6/bookings
```

### Update customer information
```http
PUT /api/Customers/3fa85f64-5717-4562-b3fc-2c963f66afa6
Content-Type: application/json

{
  "firstName": "Jane",
  "lastName": "Smith-Johnson",
  "email": "jane.johnson@example.com",
  "phoneNumber": "+1-555-9876",
  "address": "789 Park Avenue",
  "city": "San Francisco",
  "country": "USA"
}
```

### Deactivate a customer account
```http
POST /api/Customers/3fa85f64-5717-4562-b3fc-2c963f66afa6/deactivate
```

---

## Security Considerations

1. **Authentication Required:**
   - All endpoints should require authentication
   - Customers should only access their own data
   - Admin roles needed for viewing other customers

2. **Data Protection:**
   - Implement proper authorization checks
   - Mask sensitive information in logs
   - Follow GDPR/privacy regulations

3. **Rate Limiting:**
   - Implement rate limiting on customer creation
   - Prevent brute force attacks on email lookup
   - Monitor for suspicious activity

4. **Input Validation:**
   - Sanitize all input data
   - Validate email format thoroughly
   - Check for SQL injection attempts
   - Validate phone number formats

---

## Integration Points

### Related Services:
- **Booking Service:** For managing customer bookings
- **Loyalty Service:** For managing loyalty membership and points
- **Authentication Service:** For customer login and session management
- **Notification Service:** For sending booking confirmations and updates

### Related Controllers:
- [HotelBookingController](./HotelBookingController-API.md) - For creating and managing bookings
- [HotelsController](./HotelsController-API.md) - For browsing hotels
- [RoomsController](./RoomsController-API.md) - For checking room availability

---

## Notes

- All endpoints use JSON for request and response bodies
- All GUIDs should be in standard format
- Email addresses are case-insensitive
- Phone numbers can be in any format but should include country code
- The loyalty program features require separate service integration
- Consider implementing email verification for new customer registrations
