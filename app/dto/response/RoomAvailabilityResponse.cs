namespace hotel_booking.dto.response;

public record RoomAvailabilityResponse
{
    public Guid RoomId { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateOnly CheckInDate { get; init; }
    public DateOnly CheckOutDate { get; init; }
    public int TotalCount { get; init; }
    public int BookedCount { get; init; }
    public int AvailableCount { get; init; }
}
