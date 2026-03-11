namespace HotelBooking.EndPoint.Common.Models;

public abstract class Base
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
