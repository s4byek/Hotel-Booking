namespace hotel_booking.dto.request;

public record CreateBookingRequest
{
    public Guid GuestId { get; init; }
    public DateOnly CheckInDate { get; init; }
    public DateOnly CheckOutDate { get; init; }
    public IReadOnlyList<Guid> RoomIds { get; init; } = [];
}
