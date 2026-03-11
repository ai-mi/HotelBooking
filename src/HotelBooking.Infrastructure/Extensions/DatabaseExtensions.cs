using HotelBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBooking.Infrastructure.Extensions;

public static class DatabaseExtensions
{
    /// <summary>
    /// Applies any pending migrations and creates the database if it doesn't exist
    /// </summary>
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<HotelBookingDbContext>();

        // Apply any pending migrations
        await context.Database.MigrateAsync();
    }

    ///// <summary>
    ///// Seeds the database with sample data (for development/testing)
    ///// </summary>
    //public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
    //{
    //    using var scope = serviceProvider.CreateScope();
    //    var context = scope.ServiceProvider.GetRequiredService<HotelBookingDbContext>();

    //    // Check if database already has data
    //    if (await context.Hotels.AnyAsync())
    //    {
    //        return; // Database already seeded
    //    }

    //    // Add sample data here if needed
    //    // Example:
    //    // var hotel = new Hotel { Name = "Sample Hotel", ... };
    //    // context.Hotels.Add(hotel);
    //    // await context.SaveChangesAsync();
    //}
}
