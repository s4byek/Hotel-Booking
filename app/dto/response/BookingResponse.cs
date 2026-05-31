namespace hotel_booking.dto.response;

public record BookingResponse
{
    public Guid Id { get; init; }
    public Guid GuestId { get; init; }
    public string GuestName { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateOnly CheckInDate { get; init; }
    public DateOnly CheckOutDate { get; init; }
    public int Nights { get; init; }
    public decimal Total { get; init; }
    public Guid[] RoomIds { get; init; } = [];
    public int EarnedPoints { get; init; }
}
