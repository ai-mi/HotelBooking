# Hotel Booking API

A comprehensive hotel booking platform API built with .NET 8, featuring room management, bookings, payment processing, and a loyalty program.

## 🏨 Overview

This is a RESTful API for managing hotel bookings with support for:
- Hotel and room management
- Customer registration and management
- Booking system with availability checks
- Payment processing
- Loyalty program with points and transactions
- Comprehensive audit logging

## 🚀 Technologies

- **.NET 8** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core** - ORM for database operations
- **SQLite** - Lightweight database
- **Swagger/OpenAPI** - API documentation
- **Repository Pattern** - Data access abstraction
- **Unit of Work Pattern** - Transaction management

## 📁 Project Structure

```
HotelBooking/
├── src/
│   ├── HotelBooking/                      # Main API project
│   │   ├── Controllers/                   # API endpoints
│   │   └── Program.cs                     # Application entry point
│   ├── HotelBooking.Infrastructure/       # Data access layer
│   │   ├── Data/                          # DbContext and seed data
│   │   ├── Repositories/                  # Repository implementations
│   │   └── Migrations/                    # EF Core migrations
│   ├── HotelBooking.EndPoint.Common/      # Shared contracts
│   │   └── Interfaces/                    # Service interfaces
│   └── HotelBooking.External/             # External services
│       └── Services/                      # Service implementations
└── README.md
```

## 📋 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

## 🛠️ Setup Instructions

### 1. Clone the Repository

```bash
git clone <repository-url>
cd HotelBooking
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Database Setup

The application uses SQLite and will automatically:
- Create the database on first run
- Apply migrations
- Seed initial data

The database file will be created as `HotelBooking.db` in the API project directory.

### 4. Build the Solution

```bash
dotnet build
```

## ▶️ Running the Application

### Using .NET CLI

```bash
cd src/HotelBooking
dotnet run
```

### Using Visual Studio

1. Open the solution in Visual Studio
2. Set `HotelBooking` as the startup project
3. Press `F5` or click the "Start" button

### Access the Application

Once running, the application will be available at:
- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`
- **Swagger UI**: `https://localhost:5001` (root path)

## 📚 API Documentation

The API documentation is automatically generated using Swagger/OpenAPI and is available at the root URL when the application is running.

### Available Endpoints

The API provides endpoints for:
- **Hotels** - Manage hotel information
- **Rooms** - Room inventory and management
- **Customers** - Customer registration and profiles
- **Bookings** - Create and manage reservations
- **Payments** - Process and track payments
- **Loyalty Program** - Member management and points tracking

## 🗄️ Database Schema

The application includes the following entities:
- **Hotel** - Hotel properties
- **Room** - Room inventory with types and pricing
- **Customer** - Customer information
- **Booking** - Reservation records
- **Payment** - Payment transactions
- **LoyaltyMember** - Loyalty program members
- **LoyaltyTransaction** - Points transactions
- **Audit Logs** - Room, Booking, and Customer audit trails

## 🔧 Configuration

### Connection String

The default connection string is configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=HotelBooking.db"
  }
}
```

### CORS Configuration

CORS is configured to allow all origins in development. For production, update the CORS policy in `Program.cs` to restrict allowed origins.

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 🔄 Database Migrations

### Create a New Migration

```bash
cd src/HotelBooking.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../HotelBooking
```

### Apply Migrations

Migrations are automatically applied on application startup. To manually apply:

```bash
cd src/HotelBooking
dotnet ef database update --project ../HotelBooking.Infrastructure
```

### Remove Last Migration

```bash
cd src/HotelBooking.Infrastructure
dotnet ef migrations remove --startup-project ../HotelBooking
```

## 🏗️ Architecture

The solution follows a layered architecture:

- **API Layer** - Controllers and API configuration
- **Infrastructure Layer** - Data access, repositories, and EF Core context
- **Common Layer** - Shared interfaces and contracts
- **External Layer** - External service integrations

### Design Patterns

- **Repository Pattern** - Abstracts data access logic
- **Unit of Work** - Manages transactions across multiple repositories
- **Dependency Injection** - Loose coupling and testability

## 🔐 Security Considerations

For production deployment, consider:
- Adding authentication (JWT, OAuth, etc.)
- Implementing authorization policies
- Restricting CORS to specific origins
- Using HTTPS only
- Implementing rate limiting
- Adding input validation and sanitization
- Securing sensitive configuration data

Time spent: ~ 4.5 hours

## Technology Stack Recommendations

### Recommended Stack (Current → Future)

| Layer | Current | Recommended | Rationale |
|-------|---------|-------------|-----------|
| **API Framework** | ASP.NET Core | ASP.NET Core 8 | ✅ Keep - Modern, performant |
| **Database** | SQLite | PostgreSQL | ⚠️ Upgrade - Production scalability |
| **ORM** | EF Core | EF Core 8 | ✅ Keep - Good fit |
| **Caching** | None | Redis | 🔴 Add - Essential for scale |
| **Message Queue** | None | RabbitMQ / Azure Service Bus | 🟡 Add - Async processing |
| **Auth** | None | Azure AD B2C / Auth0 | 🔴 Add - Security requirement |
| **Logging** | Basic | Serilog + Seq | 🟡 Add - Observability |
| **Monitoring** | None | Application Insights / Grafana | 🔴 Add - Production monitoring |
| **API Gateway** | None | Azure API Management / Kong | 🟡 Add - For scale |
| **Container** | None | Docker + Kubernetes | 🟡 Add - For cloud deployment |

## Migration & Evolution Strategy

### Phase 1: Foundation (Months 1-3)
**Goal:** Make current system production-ready

- ✅ Migrate SQLite → PostgreSQL
- ✅ Add authentication & authorization
- ✅ Implement comprehensive logging
- ✅ Add health checks
- ✅ Set up CI/CD pipeline
- ✅ Add unit & integration tests (80% coverage)
- ✅ Implement API versioning
- ✅ Add input validation
- ✅ Set up monitoring (Application Insights)
- ✅ Documentation (OpenAPI, README)

**Effort:** 240 hours (2 devs × 6 weeks)
**Cost:** $30,000

### Phase 2: Scale (Months 4-6)
**Goal:** Prepare for growth

- ✅ Add Redis caching layer
- ✅ Implement rate limiting
- ✅ Add database read replicas
- ✅ Implement async processing (message queue)
- ✅ Add CDN for static content
- ✅ Optimize database queries
- ✅ Implement connection pooling
- ✅ Add load testing
- ✅ Set up auto-scaling

**Effort:** 320 hours (2 devs × 8 weeks)
**Cost:** $40,000

### Phase 3: Modularize (Months 7-12)
**Goal:** Prepare for team growth

- ✅ Refactor to modular monolith
- ✅ Define module boundaries
- ✅ Implement domain events
- ✅ Add CQRS for read-heavy operations
- ✅ Separate database schemas per module
- ✅ Implement module-level testing
- ✅ Document module contracts

**Effort:** 480 hours (3 devs × 8 weeks)
**Cost:** $60,000

### Phase 4: Distribute (Months 13-18) - Optional
**Goal:** Enable independent scaling

- Extract Payment Service
- Extract Notification Service
- Extract Reporting Service
- Implement API Gateway
- Set up service mesh
- Implement distributed tracing
- Add service-to-service auth

**Effort:** 960 hours (4 devs × 12 weeks)
**Cost:** $120,000

**Total Investment:** $250,000 over 18 months

---

## Decision Records

### Key Architecture Decisions

| # | Decision | Status | Date | Rationale |
|---|----------|--------|------|-----------|
| 1 | Use Layered Architecture Initially | ✅ Accepted | 2024-01 | Simple, fast to develop |
| 2 | Migrate from SQLite to PostgreSQL | ✅ Accepted | 2024-01 | Production scalability |
| 3 | Adopt Modular Monolith Pattern | 📋 Proposed | 2024-02 | Prepare for growth |
| 4 | Implement CQRS for Reads | 📋 Proposed | 2024-03 | Optimize query performance |
| 5 | Extract Payment Service | ⏳ Deferred | TBD | Wait for scale need |
| 6 | Use Microservices Architecture | ❌ Rejected | 2024-01 | Too complex for current needs |
| 7 | Add Redis Caching | ✅ Accepted | 2024-02 | Performance requirement |
| 8 | Use Azure for Hosting | ✅ Accepted | 2024-01 | Enterprise requirements |

---

## Conclusion

This hotel booking system can evolve from a simple layered monolith to a sophisticated distributed system as business needs dictate. The recommended approach is:

1. **Start Simple** - Enhanced layered monolith with proper NFRs
2. **Modularize** - Prepare for growth with modular monolith
3. **Distribute Selectively** - Extract services only when clear benefit

**Key Success Factors:**
- Pragmatic approach (don't over-engineer)
- Focus on business value
- Invest in observability early
- Automate everything
- Leverage AI for productivity
- Document decisions
- Regular architecture reviews
- Continuous refactoring

**Anti-Patterns to Avoid:**
- Premature microservices
- Under-investment in monitoring
- Skipping authentication
- Ignoring technical debt
- No deployment automation
- Poor documentation
