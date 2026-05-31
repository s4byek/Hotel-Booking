namespace hotel_booking.dto.response;

public record GuestLoyaltyResponse
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public int LoyaltyPoints { get; init; }
}
