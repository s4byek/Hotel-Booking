namespace hotel_booking.model;

public class Booking
{
    public Guid Id { get; set; }

    public Guid GuestId { get; set; }

    public string GuestName { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public DateOnly CheckInDate { get; set; }

    public DateOnly CheckOutDate { get; set; }

    public int Nights { get; set; }

    public decimal Total { get; set; }

    public Guid[] RoomIds { get; set; } = [];

    public int EarnedPoints { get; set; }
}
