namespace hotel_booking.dto.response;

public record RoomResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal NightlyRate { get; init; }
    public int Capacity { get; init; }
    public int AvailableCount { get; init; }
}
