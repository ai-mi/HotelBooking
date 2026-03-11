# HotelBooking.External

## Purpose
This class library project is designed to contain external service integrations and third-party API interactions for the Hotel Booking system.

## Project Information
- **Target Framework**: .NET 8.0
- **Project Type**: Class Library
- **Nullable Reference Types**: Enabled
- **Implicit Usings**: Enabled

## Typical Use Cases

This project should contain:

### 1. External Service Integrations
- Payment gateway integrations (Stripe, PayPal, etc.)
- Email service providers (SendGrid, Amazon SES, etc.)
- SMS notification services (Twilio, etc.)
- Third-party booking systems integration

### 2. API Clients
- Custom HTTP clients for external APIs
- SDK wrappers for third-party services
- Authentication/authorization helpers for external services

### 3. External Data Providers
- Currency conversion services
- Location/mapping services
- Weather APIs
- Review/rating aggregators

## Recommended Structure

```
HotelBooking.External/
├── Services/
│   ├── Payment/
│   │   ├── IPaymentService.cs
│   │   ├── StripePaymentService.cs
│   │   └── PayPalPaymentService.cs
│   ├── Notification/
│   │   ├── IEmailService.cs
│   │   ├── ISmsService.cs
│   │   └── Implementations/
│   └── Booking/
│       └── External booking system integrations
├── Models/
│   └── DTOs for external services
├── Configuration/
│   └── Service configuration classes
└── Extensions/
    └── Dependency injection extensions
```

## Dependencies

Add external service SDKs and packages as needed:
```xml
<ItemGroup>
  <!-- Example packages -->
  <!-- <PackageReference Include="Stripe.net" Version="x.x.x" /> -->
  <!-- <PackageReference Include="Twilio" Version="x.x.x" /> -->
  <!-- <PackageReference Include="SendGrid" Version="x.x.x" /> -->
</ItemGroup>
```

## Usage Example

### Service Interface
```csharp
namespace HotelBooking.External.Services.Payment;

public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
    Task<RefundResult> RefundPaymentAsync(string transactionId);
}
```

### Service Implementation
```csharp
namespace HotelBooking.External.Services.Payment;

public class StripePaymentService : IPaymentService
{
    // Implementation
}
```

### Registration in Startup Project
```csharp
// In Program.cs
builder.Services.AddScoped<IPaymentService, StripePaymentService>();
```

## Best Practices

1. **Interface Segregation**: Define clear interfaces for all external services
2. **Configuration**: Use options pattern for service configuration
3. **Error Handling**: Implement robust error handling and retry logic
4. **Logging**: Add comprehensive logging for external service calls
5. **Testing**: Create mock implementations for unit testing
6. **Security**: Never commit API keys or secrets to source control

## Configuration Management

Store external service credentials in:
- **Development**: User Secrets (`dotnet user-secrets`)
- **Production**: Azure Key Vault or environment variables

Example appsettings structure:
```json
{
  "ExternalServices": {
    "Stripe": {
      "ApiKey": "sk_...",
      "PublishableKey": "pk_..."
    },
    "SendGrid": {
      "ApiKey": "SG..."
    }
  }
}
```

## Notes

- Keep this project independent from the database layer (Infrastructure)
- Reference this from the Endpoint project to use external services
- Consider creating a separate interfaces library if you need cross-project contracts
