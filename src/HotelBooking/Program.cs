using HotelBooking.EndPoint.Common.Interfaces;
using HotelBooking.External.Interfaces;
using HotelBooking.External.Services;
using HotelBooking.Infrastructure.Data;
using HotelBooking.Infrastructure.Extensions;
using HotelBooking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new()
	{
		Title = "Hotel Booking API",
		Version = "v1",
		Description = "A comprehensive hotel booking platform API with room management, bookings, and loyalty program"
	});
});

// Configure SQLite Database
builder.Services.AddDbContext<HotelBookingDbContext>(options =>
	options.UseSqlite(
		builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=HotelBooking.db",
		b => b.MigrationsAssembly("HotelBooking.Infrastructure")));

// Register repositories and Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Application Services

builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ILoyaltyService, LoyaltyService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IHotelService, HotelService>();

// Configure CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll",
		builder =>
		{
			builder.AllowAnyOrigin()
				   .AllowAnyMethod()
				   .AllowAnyHeader();
		});
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel Booking API V1");
		c.RoutePrefix = string.Empty; // Swagger UI at root
	});
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		var context = services.GetRequiredService<HotelBookingDbContext>();

		// Apply migrations
		await context.Database.MigrateAsync();

		// Seed data
		await SeedData.SeedAsync(context);

		Console.WriteLine("Database initialized successfully!");
	}
	catch (Exception ex)
	{
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred while initializing the database.");
	}
}

app.Run();
