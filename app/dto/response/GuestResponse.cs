namespace hotel_booking.dto.response;

public record GuestResponse
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public int LoyaltyPoints { get; init; }
}
